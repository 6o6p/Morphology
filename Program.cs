using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Morphology.Utils;

namespace Morphology
{
    public static class Program
    {
        public static string DictionaryPath => Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            @"Resources\dict.opcorpora.txt"
        );
        
        private static void Main()
        {
            var sw = Stopwatch.StartNew();
            var morpher = SentenceMorpher.Create(FileLinesEnumerable.Create(DictionaryPath));
            Console.WriteLine($"Init took {sw.Elapsed}");
            RunLoop(morpher);
            RunOtherLoop(morpher);
        }

        public static void RunLoop(SentenceMorpher morpher)
        {
            var input = "мама{noun,anim,femn,sing,gent} мыла РАМА{noun,inan,femn,sing,accs}";
            var sw = new Stopwatch();
            do
            {
                sw.Restart();
                var result = morpher.Morph(input);
                Console.WriteLine($"[took {sw.Elapsed}]   {result}");
            } while ((input = Console.ReadLine()) is { Length: > 0 });
        }

        public static void RunOtherLoop(SentenceMorpher morpher)
        {
            var input = "ОДНАЖДЫ{ADVB} В{NOUN,anim,ms-f,Sgtm,Fixd,Abbr,Init,nomn} СТУДЁНЫЙ{ADJF,Qual,femn,sing,accs} ЗИМНИЙ{ADJF,femn,accs} ПОРА{sing,accs} Я{NOUN,anim,ms-f,Sgtm,Fixd,Abbr,Patr,Init,sing,nomn} ИЗА{NOUN,anim,plur,gent} ЛЕСА{NOUN,inan,femn,sing,accs} ВЫШЕЛ{VERB,perf,intr,sing,indc} ЕСТЬ{VERB,impf,intr,masc,sing,past,indc} СИЛЬНЫЙ{Qual,masc,nomn} МОРОЗ{anim,femn,Sgtm,Surn,sing,nomn} ГЛЯЖУ{VERB,impf,tran,sing,pres,indc} ПОДНИМАЮСЬ{VERB,impf,intr,3per,pres,indc} МЕДЛЕН{Qual,neut} В{NOUN,ms-f,Fixd,Abbr,Patr,Init,sing,nomn} ГОРА{NOUN,inan,femn,sing,accs} ЛОШАДКА{NOUN,anim,femn,sing} ВЕЗУЩИЙ{impf,pres,actv,femn,sing,nomn} ХВОРОСТ{NOUN,gen2} ВОЗ{NOUN,femn,Fixd,Abbr,Orgn,sing,nomn}";
            var sw = new Stopwatch();
            do
            {
                sw.Restart();
                var result = morpher.Morph(input);
                Console.WriteLine($"[took {sw.Elapsed}]   {result}");
            } while ((input = Console.ReadLine()) is { Length: > 0 });
        }
    }
}