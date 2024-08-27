namespace Shared.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };

            AddRange(items);
        }

        //Можно использовать, когда мало зписей в БД. В этом случае мы запрашиваем всю коллекцию из базы,
        //пробрасываем в этот метод и формируем страницу
        //на стороне сервера, а не на стороне базы данных. Если записей много, то сначала запрашиваем "страницу" из базы,
        //затем запрашиваем общее количество записей и создаем PagedList<T> через конструктор
        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
