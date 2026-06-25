using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegatesEvents
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Возвращает максимальный элемент коллекции на основе заданного преобразования в число.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции (ссылочный тип).</typeparam>
        /// <param name="collection">Коллекция элементов.</param>
        /// <param name="convertToNumber">Делегат, преобразующий элемент в float.</param>
        /// <returns>Элемент с максимальным значением.</returns>
        /// <exception cref="ArgumentNullException">Если коллекция или делегат равны null.</exception>
        /// <exception cref="InvalidOperationException">Если коллекция пуста или все элементы null.</exception>
        public static T GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (convertToNumber == null)
                throw new ArgumentNullException(nameof(convertToNumber));

            T maxItem = null;
            float? maxValue = null;

            foreach (T item in collection)
            {
                if (item == null) continue; // пропускаем null-элементы
                float value = convertToNumber(item);
                if (maxValue == null || value > maxValue)
                {
                    maxValue = value;
                    maxItem = item;
                }
            }

            if (maxItem == null)
                throw new InvalidOperationException("Коллекция не содержит элементов или все элементы равны null.");

            return maxItem;
        }
    }
}
