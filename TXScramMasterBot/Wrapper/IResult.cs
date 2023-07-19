using System.Text.Json.Serialization;

namespace TXScramMasterBot.Wrapper;

public interface IResult
{
	[JsonPropertyName("messages")]
	List<string> Messages { get; set; }

	[JsonPropertyName("succeded")]
	bool Succeeded { get; set; }
}

public interface IResult<out T> : IResult
{
	[JsonPropertyName("data")]
	T Data { get; }
}