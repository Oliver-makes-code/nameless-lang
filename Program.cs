using Lang.Tokenize;

foreach (var token in Tokenizer.Tokenize(File.ReadAllText("test.txt")))
    Console.WriteLine(token);
