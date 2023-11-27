using Common.Model;

namespace Common
{
    public interface ISqlExecuter
    {
        SqlResult Execute(string sqlQuery);
    }
}
