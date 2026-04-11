using System.Text.Json.Serialization;

namespace Hinario.Domain.Models
{
    public class HinoImportDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("artist")]
        public string? Artist { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("copyright")]
        public string? Copyright { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("bpm")]
        public double? Bpm { get; set; }

        [JsonPropertyName("time_sig")]
        public string? TimeSig { get; set; }

        [JsonPropertyName("midi")]
        public object? Midi { get; set; }

        [JsonPropertyName("order")]
        public string? Order { get; set; }

        [JsonPropertyName("arrangements")]
        public List<object>? Arrangements { get; set; }

        [JsonPropertyName("lyrics")]
        public LyricsDto? Lyrics { get; set; }

        [JsonPropertyName("streaming")]
        public StreamingDto? Streaming { get; set; }

        [JsonPropertyName("extras")]
        public ExtrasDto? Extras { get; set; }
    }

    public class LyricsDto
    {
        [JsonPropertyName("full_text")]
        public string? FullText { get; set; }

        [JsonPropertyName("full_text_with_comment")]
        public string? FullTextWithComment { get; set; }

        [JsonPropertyName("paragraphs")]
        public List<ParagraphDto>? Paragraphs { get; set; }
    }

    public class ParagraphDto
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("text_with_comment")]
        public string? TextWithComment { get; set; }

        [JsonPropertyName("translations")]
        public object? Translations { get; set; }
    }

    public class StreamingDto
    {
        [JsonPropertyName("audio")]
        public AudioDto? Audio { get; set; }

        [JsonPropertyName("backing_track")]
        public BackingTrackDto? BackingTrack { get; set; }
    }

    public class AudioDto
    {
        [JsonPropertyName("spotify")]
        public string? Spotify { get; set; }

        [JsonPropertyName("youtube")]
        public string? Youtube { get; set; }

        [JsonPropertyName("deezer")]
        public string? Deezer { get; set; }
    }

    public class BackingTrackDto
    {
        [JsonPropertyName("spotify")]
        public string? Spotify { get; set; }

        [JsonPropertyName("youtube")]
        public string? Youtube { get; set; }

        [JsonPropertyName("deezer")]
        public string? Deezer { get; set; }
    }

    public class ExtrasDto
    {
        [JsonPropertyName("extra")]
        public string? Extra { get; set; }
    }
}

