using JYShop.Common.DB;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace JYShopModel
{
    public class MyContext
    {
        private static string _connectionString = BaseDBConfig.ConnectionString;
        private static DbType _dbType = (DbType)BaseDBConfig.DbType;
        private SqlSugarClient _db;

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

        public static MyContext Context
        {
            get { return new MyContext(); }
        }

        public MyContext()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException("数据库连接字符串为空");
            }
            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = true,
                IsShardSameThread = false,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService=new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery=true,
                    IsAutoRemoveDataCache = true
                }
            });
        }

        public SimpleClient<T> GetEntityDB<T>() where T : class, new()
        {
            return new SimpleClient<T>(_db);
        }

        public SimpleClient<T> GetEntityDB<T>(SqlSugarClient sugarClient) where T : class, new()
        {
            return new SimpleClient<T>(sugarClient);
        }


        public void CreateClassFileByDBTable(string strPath)
        {
            CreateClassFileByDBTable(strPath, "Shop.Core.Entity");
        }

        public void CreateClassFileByDBTable(string strPath, string strNameSpace)
        {
            CreateClassFileByDBTable(strPath, strNameSpace, null);
        }

        public void CreateClassFileByDBTable(string strPath, string strNameSpace, string[] lstTableNames)
        {
            CreateClassFileByDBTable(strPath, strNameSpace, lstTableNames, string.Empty);
        }

        public void CreateClassFileByDBTable(string strPath, string strNameSpace, string[] lstTableNames, string strInterFace, bool blnSerializable = false)
        {

            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                _db.DbFirst.Where(lstTableNames).IsCreateDefaultValue().IsCreateAttribute().SettingClassTemplate(p => p = @"
{using}
namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (blnSerializable ? "[Serializable]" : "") + @"
     public partial class {ClassName}" + (string.IsNullOrEmpty(strInterFace) ? "" : (" : " + strInterFace)) + @"
       
    {
        public {ClassName}()
        {
         {Constructor}
        }
{PropertyName}
    }
}
").SettingPropertyTemplate(q => q = @"
            {SugarColumn}
            public {PropertyType} {PropertyName}
            {
                get{
                    return {PropertyName};
                }
                set{
                    if(_{PropertyName}!=value)
                        {
                            base.SetValueCall(" + "\"{PropertyName}\",_{PropertyName}" + @")；
                        }
                        _{PropertyName}=value;
                    }
            }").SettingPropertyDescriptionTemplate(p => p = "     private {PropertyType} _{PropertyName};\r\n" + p)
            .SettingConstructorTemplate(p => p = "     this._{PropertyName}={DefaultValue};")
            .CreateClassFile(strPath, strNameSpace);
            }
            else
            {

                _db.DbFirst.IsCreateAttribute().IsCreateDefaultValue()
                   .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (blnSerializable ? "[Serializable]" : "") + @"
    public partial class {ClassName}" + (string.IsNullOrEmpty(strInterFace) ? "" : (" : " + strInterFace)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                   .SettingPropertyTemplate(p => p = @"
            {SugarColumn}
            public {PropertyType} {PropertyName}
            {
                get
                {
                    return _{PropertyName};
                }
                set
                {
                    if(_{PropertyName}!=value)
                    {
                        base.SetValueCall(" + "\"{PropertyName}\",_{PropertyName}" + @");
                    }
                    _{PropertyName}=value;
                }
            }")
                   .SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                   .SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                   .CreateClassFile(strPath, strNameSpace);
            }
        }



        public void CreateTableByEntity<T>(bool blnBackupTable, params T[] lstEntitys) where T : class, new()
        {
            Type[] lstTypes = null;
            if (lstEntitys != null)
            {
                lstTypes = new Type[lstEntitys.Length];
                for (var i = 0; i < lstEntitys.Length; i++)
                {
                    T t = lstEntitys[i];
                    lstTypes[i] = typeof(T); 
                } 
            }
            CreateTableByEntity(blnBackupTable, lstTypes);
        }

        public void CreateTableByEntity(bool blnBackupTable, params Type[] lstEntitys)
        {
            if (blnBackupTable)
            {
                _db.CodeFirst.BackupTable().InitTables(lstEntitys);
            }
            else {
                _db.CodeFirst.InitTables(lstEntitys);
            }
        }



        public static MyContext GetDBContext()
        {
            return new MyContext();
        }

        public static void Init(string strConnectionString, DbType enmDBType = SqlSugar.DbType.SqlServer)
        {
            _connectionString = strConnectionString;
            _dbType = enmDBType;
        }

        public static ConnectionConfig GetConnectionConfig(bool blnIsAutoCloseConnection = true, bool blnIsShardSameThread = false)
        {
            ConnectionConfig config = new ConnectionConfig {
                 ConnectionString=_connectionString,
                 DbType=_dbType,
                 IsAutoCloseConnection=blnIsAutoCloseConnection,
                 IsShardSameThread=blnIsShardSameThread,
                 ConfigureExternalServices =new ConfigureExternalServices {

                 }
                 //,
                 //MoreSettings =new ConnMoreSettings {
                 //     IsAutoRemoveDataCache=true,
                 //      IsWithNoLockQuery=true,
                 //}
            };

            return config;
        }


        public static SqlSugarClient GetCustomDB(ConnectionConfig config)
        {
            return new SqlSugarClient(config);
        }

        public static SimpleClient<T> GetCustomEntityDB<T>(SqlSugarClient client) where T : class, new()
        {
            return new SimpleClient<T>(client);
        }

        public static SimpleClient<T> GetCustomEntityDB<T>(ConnectionConfig config) where T : class, new()
        {
            SqlSugarClient client = GetCustomDB(config);
            return new SimpleClient<T>(client); 

        }
    }
}
