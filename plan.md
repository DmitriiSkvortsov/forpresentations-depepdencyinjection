# план рассказа
SOLID -> S мелкие классы зависящие друг от друга + D куча абстракций -> сложно создать конкретный класс -> DI + new() страшный сон -> DI Container

общие моменты -> общие положения -> паттерны -> практики -> плюшки -> практика

# общие положения
- виды инъекций
- lifestyles (singleton, transient, scoped, special: per graph)

# паттеры использования контейнера
- Composition Root – это (предпочтительно) уникальное местоположение в приложении, где модули соединяются друг с другом
- RRR - единая последовательность

# практики:
- работа с настройками (inject settings)
- composite и регистрация коллекций (+ коллекции в конструкторе)
- assert+diagnostics: Lifestyle Mismatches, Short-Circuited Dependencies, Torn/Ambiguous Lifestyles, Disposable Transient Components
- именованные объекты / conditional
- декорирование / перехват - общие декораторы
- generic + autoscanning
- ambient context (time/logging - with default local) + CCC
- provider + factory
- fallback: ResolveUnregisteredType

# плюшки
- AOP

# практика
- решение проблемы тестирования по макс количеству заказов в пуле Ярика
- кеширование catalogClient


# ссылки
https://simpleinjector.readthedocs.io/en/latest/quickstart.html#overview
марк симан - внедрение зависимостей в .net





public interface ICatalogClient 
{
    OfferEntry[] LoadAll(int[] ids);
}

public class CachedCatalogClientDecorator : ICatalogClient
{
    private readonly ICatalogCache _cache;

    public CachedCatalogClientDecorator(ICatalogClient original, ICatalogCache catalogCache)
    {

    }

    public OfferEntry[] LoadAll(int[] ids)
    {
        cache...
    }
} 

container.Register<ICatalogClient, ...>()
container.Register<ICatalogCache, ...>(Singleton)
container.RegisterDecorator<ICatalogClient, CachedCatalogClientDecorator>