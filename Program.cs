using System;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace CompilationOptionsReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("Expected 1 argument: path to pdb to analyze");

            // Find the embedded pdb
            using var fileStream = File.OpenRead(args[0]);
            using var peReader = new PEReader(fileStream);
            var pdbOpenedByReader = peReader.TryOpenAssociatedPortablePdb(
                args[0],
                s => File.OpenRead(s),
                out var pdbReaderProvider,
                out var pdbPath);

            if (!pdbOpenedByReader || pdbReaderProvider is null)
            {
                throw new Exception($"Could not open {args[0]} pdb");
            }

            var reader = pdbReaderProvider.GetMetadataReader();
            var compilationOptionsReader = new CompilationOptionsReader(reader);

            var compilationOptions = compilationOptionsReader.GetCompilationOptions();

            Console.WriteLine("=====OPTIONS=====");
            foreach (var option in compilationOptions.Options.OrderBy((kvp) => kvp.optionName))
            {
                Console.WriteLine($"{option.optionName} := {option.value}");
            }

            Console.WriteLine();
            Console.WriteLine("======REFERENCES=====");
            foreach (var reference in compilationOptionsReader.GetMetadataReferences())
            {
                Console.WriteLine($"{reference.Name} => ({reference.Timestamp}, {reference.ImageSize}, {reference.Mvid})");
            }
        }
    }
}
