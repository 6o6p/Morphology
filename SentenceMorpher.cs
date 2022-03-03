using System;
using System.Collections.Generic;
using System.Linq;

namespace Morphology
{
    public class WordWithAttributes
    {
        public string Word { get; }
        public HashSet<string> Attributes { get; }

        public WordWithAttributes(string word, HashSet<string> attributes)
        {
            Word = word;
            Attributes = attributes;
        }
    }

    public class SentenceMorpher
    {
        private readonly Dictionary<string, List<WordWithAttributes>> _dictionary;
        public SentenceMorpher(Dictionary<string, List<WordWithAttributes>> dic) =>_dictionary = dic;

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
            var dictionary = new Dictionary<string, List<WordWithAttributes>>();
            var isMainForm = false;
            var wordMainForm = string.Empty;

            foreach (var line in dictionaryLines.Where(line => line.Length != 0))
            {
                if (int.TryParse(line, out _))
                {
                    isMainForm = true;
                    continue;
                }

                var wordForm = ParseLine(line);
                if (isMainForm)
                {
                    wordMainForm = wordForm.Word;
                    isMainForm = false;
                }

                if (!dictionary.ContainsKey(wordMainForm))
                {
                    dictionary[wordMainForm] = new List<WordWithAttributes>();
                }

                dictionary[wordMainForm].Add(wordForm);
            }
            return new SentenceMorpher(dictionary);
        }

        public static WordWithAttributes ParseLine(string line)
        {
            var arr = line.Split('\t');

            var word = arr[0].ToUpper();
            var attributes = arr[1].ToUpper().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            return new WordWithAttributes(word, new HashSet<string>(attributes));
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
                var temp = word.ToUpper().Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                var requiredWord = temp[0];

                if (temp.Length == 1)
                {
                    result.Add(requiredWord);
                    continue;
                }

                var attributes = temp[1].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (_dictionary.TryGetValue(requiredWord, out var wordForms))
                {
                    foreach (var wordForm in wordForms)
                    {
                        var isRequiredForm = true;
                        foreach (var attribute in attributes)
                        {
                            if (!wordForm.Attributes.Contains(attribute))
                            {
                                isRequiredForm = false;
                                break;
                            }
                        }

                        if (isRequiredForm)
                        {
                            requiredWord = wordForm.Word;
                            break;
                        }
                    }
                }
                result.Add(requiredWord);
            }
            return string.Join(' ', result);
        }
    }
}
