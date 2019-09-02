using JYShop.Common.Log;
using JYShop.Common.LogHelper;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JYShop.Repository
{
    public class DbContext
    {
        private static string _connectionString;
        private static DbType _dbType;
        private static SqlSugarClient _db;
        private readonly ILoggerHelper _loggerHelper = new LogHelper();

        /// <summary>
        /// 连接字符串 
        /// Blog.Core
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        /// <summary>
        /// 数据库类型 
        /// Blog.Core 
        /// </summary>
        public static DbType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }
        /// <summary>
        /// 数据连接对象 
        /// Blog.Core 
        /// </summary>
        public SqlSugarClient Db
        {
            get { return _db; }
            private set { _db = value; }
        }

        public static DbContext Context
        {
            get { return new DbContext(); }
        }

        private DbContext(bool blnIsAutoCloseConnection = true)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException("数据库连接字符串为空");
            }

            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = blnIsAutoCloseConnection,
                IsShardSameThread = false,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //DataInfoCacheService=new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings
                {
                    // IsWithNoLockQuery=true,
                    IsAutoRemoveDataCache = true
                }
            });

            //_db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            //{
            //    OutSql2Log(sql, GetParas(pars));
            //};

            _db.Aop.OnLogExecuting = (sql, pars) =>
              {
                  Parallel.For(0, 1, e =>
                  {
                      MiniProfiler.Current.CustomTiming("SQL：", GetParas(pars) + "【SQL语句】：" + sql);
                      LogLock.OutSql2Log("SqlLog", new string[] { GetParas(pars), "【SQL语句】：" + sql });
                  });
              };

        }


        public SimpleClient<T> GetEntityDB<T>() where T : class, new()
        {
            return new SimpleClient<T>(_db);
        }

        public SimpleClient<T> GetEntityDB<T>(SqlSugarClient db) where T : class, new()
        {
            return new SimpleClient<T>(db);
        }

        private string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }



        /// <summary>
        /// 功能描述:获得一个DbContext
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="blnIsAutoCloseConnection">是否自动关闭连接（如果为false，则使用接受时需要手动关闭Db）</param>
        /// <returns>返回值</returns>
        public static DbContext GetDbContext(bool blnIsAutoCloseConnection = true)
        {
            return new DbContext(blnIsAutoCloseConnection);
        }

        /// <summary>
        /// 功能描述:设置初始化参数
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strConnectionString">连接字符串</param>
        /// <param name="enmDbType">数据库类型</param>
        public static void Init(string strConnectionString, DbType enmDbType = SqlSugar.DbType.SqlServer)
        {
            _connectionString = strConnectionString;
            _dbType = enmDbType;
        }


        public static ConnectionConfig GetConnectionConfig(bool blnIsAutoCloseConnection = true, bool blnIsShardSameThread = false)
        {
            ConnectionConfig config = new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = blnIsAutoCloseConnection,
                IsShardSameThread = blnIsShardSameThread,
                ConfigureExternalServices = new ConfigureExternalServices()
                {

                }
            };
            return config;
        }

        public static SqlSugarClient GetCustomDB(ConnectionConfig config)
        {
            return new SqlSugarClient(config);
        }

        public static SimpleClient<T> GetCustomDB<T>(SqlSugarClient sugarClient) where T : class, new()
        {
            return new SimpleClient<T>(sugarClient);
        }

        public static SimpleClient<T> GetCustomDB<T>(ConnectionConfig config) where T : class, new()
        {
            SqlSugarClient client = GetCustomDB(config);
            return GetCustomDB<T>(client);

        }
    }
}
