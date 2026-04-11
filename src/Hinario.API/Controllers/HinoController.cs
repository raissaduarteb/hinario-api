using Microsoft.AspNetCore.Mvc;
using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;

[ApiController]
[Route("api/[controller]")]
public class HinoController : ControllerBase
{
    private readonly IHinoService _hinoService;

    public HinoController(IHinoService hinoService)
    {
        _hinoService = hinoService;
    }

    [HttpGet]
    public IActionResult GetHinos() => Ok(_hinoService.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetHino(int id)
    {
        var hino = _hinoService.GetById(id);
        return hino == null ? NotFound() : Ok(hino);
    }

    [HttpGet("identificador/{identificador}")]
    public IActionResult GetHinoByIdentificador(string identificador)
    {
        if (string.IsNullOrWhiteSpace(identificador))
            return BadRequest("Identificador não pode ser vazio");

        var hino = _hinoService.GetByIdentificador(identificador);
        return hino == null ? NotFound() : Ok(hino);
    }

    [HttpGet("pesquisar")]
    public IActionResult PesquisarHinos([FromQuery] string texto)
    {
        var resultado = _hinoService.Pesquisar(texto);
        return Ok(resultado);
    }

    [HttpPost]
    public IActionResult CreateHino([FromBody] Hino hino)
    {
        _hinoService.Add(hino);
        return CreatedAtAction(nameof(GetHino), new { id = hino.Id }, hino);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateHino(int id, [FromBody] Hino hino)
    {
        if (id != hino.Id)
            return BadRequest("id do hino não corresponde ao id da rota");

        var atualizado = _hinoService.Update(id, hino);
        return atualizado == null ? NotFound() : NoContent();
    }

    [HttpGet("minerar/canticos/{numero:int}")]
    public async Task<IActionResult> MinerarCantico(int numero, CancellationToken cancellationToken = default)
    {
        if (numero < 1 || numero > 100)
            return BadRequest(new { mensagem = "O número do cântico deve estar entre 1 e 100." });

        var identificador = $"C-{numero}";
        var existente = _hinoService.GetByIdentificador(identificador);
        if (existente != null)
            return Ok(new { mensagem = "Hino já cadastrado.", hino = existente });

        var hino = await _hinoService.MinerarCantico(numero, cancellationToken);
        if (hino == null)
            return StatusCode(502, new { mensagem = "Não foi possível extrair o hino do site (página indisponível ou formato inesperado)." });

        return CreatedAtAction(nameof(GetHino), new { id = hino.Id }, hino);
    }

    [HttpPost("importar")]
    public IActionResult ImportarHinos([FromBody] List<HinoImportDto> hinosImport)
    {
        if (hinosImport == null || hinosImport.Count == 0)
            return BadRequest("Lista de hinos não pode ser vazia");

        var (importados, erros) = _hinoService.ImportarHinos(hinosImport);

        if (importados == 0)
            return BadRequest(new { mensagem = "Nenhum hino foi importado", erros });

        var resultado = new
        {
            mensagem = $"Importação concluída: {importados} hino(s) importado(s) com sucesso",
            importados,
            erros = erros.Count > 0 ? erros : null
        };

        return erros.Count > 0 ? StatusCode(207, resultado) : Ok(resultado);
    }

    [HttpGet("{tipo}/{numero}/proximo")]
    public async Task<IActionResult> GetProximo(string tipo, int numero)
    {
        var proximo = _hinoService.ObterProximo(tipo, numero);

        if (proximo != null)
            return Ok(proximo);

        return NotFound();
    }

    [HttpGet("{tipo}/{numero}/anterior")]
    public async Task<IActionResult> GetAnterior(string tipo, int numero)
    {
        var anterior = _hinoService.ObterAnterior(tipo, numero);

        if (anterior != null)
            return Ok(anterior);

        return NotFound();
    }
}
