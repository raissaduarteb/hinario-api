using Hinario.Domain.Dtos;
using Hinario.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RepertorioController(IRepertorioService repertorioService) : ControllerBase
{
    // ─── Leitura (membros) ───────────────────────────────────────────────────

    [HttpGet]
    public IActionResult GetAll() => Ok(repertorioService.GetAll());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var repertorio = repertorioService.GetById(id);
        return repertorio == null ? NotFound() : Ok(repertorio);
    }

    [HttpGet("ativo")]
    public IActionResult GetAtivo()
    {
        var repertorio = repertorioService.GetAtivo();
        return repertorio == null ? NotFound(new { mensagem = "Nenhum repertório ativo no momento." }) : Ok(repertorio);
    }

    [HttpGet("{id:int}/hinos/{ordem:int}/proximo")]
    public IActionResult GetProximo(int id, int ordem)
    {
        var item = repertorioService.GetProximoItem(id, ordem);
        return item == null ? NotFound(new { mensagem = "Não há próximo hino no repertório." }) : Ok(item);
    }

    [HttpGet("{id:int}/hinos/{ordem:int}/anterior")]
    public IActionResult GetAnterior(int id, int ordem)
    {
        var item = repertorioService.GetAnteriorItem(id, ordem);
        return item == null ? NotFound(new { mensagem = "Não há hino anterior no repertório." }) : Ok(item);
    }

    // ─── Administração ───────────────────────────────────────────────────────

    [HttpPost]
    public IActionResult Criar([FromBody] CriarRepertorioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            return BadRequest(new { mensagem = "Nome do repertório é obrigatório." });

        var repertorio = repertorioService.Criar(dto.Nome, dto.Data);
        return CreatedAtAction(nameof(GetById), new { id = repertorio.Id }, repertorio);
    }

    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] AtualizarRepertorioDto dto)
    {
        var repertorio = repertorioService.Atualizar(id, dto.Nome, dto.Data);
        return repertorio == null ? NotFound() : Ok(repertorio);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Deletar(int id)
    {
        var deletado = repertorioService.Deletar(id);
        return deletado ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/ativar")]
    public IActionResult Ativar(int id)
    {
        var ativado = repertorioService.Ativar(id);
        return ativado ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/hinos")]
    public IActionResult AdicionarHino(int id, [FromBody] AdicionarHinoRepertorioDto dto)
    {
        var repertorio = repertorioService.AdicionarHino(id, dto.HinoId);
        if (repertorio == null)
            return NotFound(new { mensagem = "Repertório ou hino não encontrado." });

        return Ok(repertorio);
    }

    [HttpDelete("{id:int}/hinos/{itemId:int}")]
    public IActionResult RemoverHino(int id, int itemId)
    {
        var repertorio = repertorioService.RemoverHino(id, itemId);
        if (repertorio == null)
            return NotFound(new { mensagem = "Repertório ou item não encontrado." });

        return Ok(repertorio);
    }

    [HttpPut("{id:int}/hinos/ordem")]
    public IActionResult ReordenarHinos(int id, [FromBody] ReordenarHinosDto dto)
    {
        if (dto.ItemIds == null || dto.ItemIds.Count == 0)
            return BadRequest(new { mensagem = "Lista de IDs de itens não pode ser vazia." });

        var repertorio = repertorioService.ReordenarHinos(id, dto.ItemIds);
        if (repertorio == null)
            return NotFound(new { mensagem = "Repertório não encontrado ou IDs inválidos." });

        return Ok(repertorio);
    }
}
