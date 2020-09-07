
namespace SharesonServer.Interface
{
    public interface IMainMenuRepositoryFunctionsWithLimit : IMainMenuRepository
    {
        bool RunServerWithLimitedFunctions();
    }
}
