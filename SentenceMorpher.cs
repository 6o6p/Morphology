using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Morphology
{
    public class WordWithAttributes
    {
        public string Word { get; }
        public string Attributes { get; }

        public WordWithAttributes(string word, string attributes)
        {
            Word = word;
            Attributes = attributes;
        }
    }

    public class SentenceMorpher
    {
        private readonly Dictionary<string, Dictionary<string, LinkedList<string>>> _dictionary;
        public SentenceMorpher(Dictionary<string, Dictionary<string, LinkedList<string>>> dic)
        {
            _dictionary = dic;
        }

        /// <summary>
        ///     Создает <see cref="SentenceMorpher"/> из переданного набора строк словаря.
        /// </summary>
        /// <remarks>
        ///     В этом методе должен быть код инициализации: 
        ///     чтение и преобразование входных данных для дальнейшего их использования
        /// </remarks>
        /// <param name="dictionaryLines">
        ///     Строки исходного словаря OpenCorpora в формате plain-text.
        ///     <code> СЛОВО(знак_табуляции)ЧАСТЬ РЕЧИ( )атрибут1[, ]атрибут2[, ]атрибутN </code>
        /// </param>
        public static SentenceMorpher Create(IEnumerable<string> dictionaryLines)
        {
            var dictionary = new Dictionary<string, Dictionary<string, LinkedList<string>>>();
            var previousIsNumber = false;
            //var previousWasEmpty = true;
            var previousWord = string.Empty;
            foreach (var line in dictionaryLines)
            {
                if (line.Length == 0)
                {
                    //previousWasEmpty = true;
                    continue;
                }

                if (char.IsNumber(line[0]))
                {
                    previousIsNumber = true;
                    continue;
                }

                var parsed = ParseLine(line);
                if (previousIsNumber)
                //if (previousWasEmpty)
                {
                    if (!dictionary.ContainsKey(parsed.Word))
                    {
                        dictionary[parsed.Word] = new Dictionary<string, LinkedList<string>>();
                    }

                    //previousWasEmpty = false;
                    previousIsNumber = false;
                    previousWord = parsed.Word;
                }

                if (!dictionary[previousWord].ContainsKey(parsed.Attributes))
                    dictionary[previousWord][parsed.Attributes] = new LinkedList<string>();

                dictionary[previousWord][parsed.Attributes].AddLast(parsed.Word);
            }
            return new SentenceMorpher(dictionary);
        }

        public static WordWithAttributes ParseLine(string line)
        {
            var result = line.Split('\t');

            return new WordWithAttributes(
                result[0].ToLowerInvariant(),
                string.Join(',', result[1].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant()
                );
        }

        /// <summary>
        ///     Выполняет склонение предложения согласно указанному формату
        /// </summary>
        /// <param name="sentence">
        ///     Входное предложение <para/>
        ///     Формат: набор слов, разделенных пробелами.
        ///     После слова может следовать спецификатор требуемой части речи (формат описан далее),
        ///     если он отсутствует - слово требуется перенести в выходное предложение без изменений.
        ///     Спецификатор имеет следующий формат: <code>{ЧАСТЬ РЕЧИ,аттрибут1,аттрибут2,..,аттрибутN}</code>
        ///     Если для спецификации найдётся несколько совпадений - используется первое из них
        /// </param>
        public virtual string Morph(string sentence)
        {
            var words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var word in words)
            {
                var temp = word.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 1)
                {
                    result.Add(temp[0].ToLowerInvariant());
                    continue;
                }

                var attributes = string.Join(',', temp[1].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
                var wordToAdd = temp[0].ToLowerInvariant();
                if (_dictionary.TryGetValue(wordToAdd, out var wordForms) && wordForms.TryGetValue(attributes, out var requiredWord))
                    wordToAdd = requiredWord.Last.Value;

                result.Add(wordToAdd);
            }

            return string.Join(' ', result);
        }
    }
}
