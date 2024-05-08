using LLMPlayground;

Console.WriteLine("Go:");

String? input = null;
var llmHelper = new NpcLlm("Kolyan");

while ((input = Console.ReadLine()) != "exit") {
    if (input == null) break;
    input = input.Trim();
    if (input.Length == 0) continue;

    var output = llmHelper.GetCompletion(input).Result;
    Console.WriteLine(output);
}

Console.WriteLine("Bye.");
