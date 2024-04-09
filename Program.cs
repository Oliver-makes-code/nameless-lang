using Lang.Tokenize;

foreach (var token in Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")))
    Console.WriteLine(token);
