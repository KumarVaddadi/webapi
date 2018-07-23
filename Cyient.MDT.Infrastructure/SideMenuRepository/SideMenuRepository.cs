using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyient.MDT.WebAPI.Core.Common;
using Cyient.MDT.WebAPI.Core.Entities.SideMenu;
using Cyient.MDT.WebAPI.Core.Interface.SideMenu;
namespace Cyient.MDT.Infrastructure.SideMenuRepository
{
    /// <summary>
    /// Repository entity for Side menu
    /// </summary>
    public class SideMenuRepository : ISideMenu
    {
        public SideMenuRepository() { }

        /// <summary>
        /// This will return the list side menu from Solution to Configuration
        /// </summary>
        /// <returns></returns>
        public MDTTransactionInfo GetSideMenu(int UserID)
        {
            MDTTransactionInfo mdt = new MDTTransactionInfo();
            IEnumerable<SolutionList> solutionLists = null;
            SolutionList solutionList = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter Status = new SqlParameter("@Status", 0);
            Status.Direction = ParameterDirection.Output;
            prm.Add(Status);
            int StatusValue = 0;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetSolutions", out StatusValue, prm).Tables[0];

            if (StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {
                    solutionLists = from d in dt.AsEnumerable()
                                    select new SolutionList
                                    {
                                        SOLUTION_ID = d.Field<int>("SOLUTION_ID"),
                                        SOLUTION_NAME = d.Field<string>("SOLUTION_NAME"),
                                        Packages = GetPackageList(d.Field<int>("SOLUTION_ID"), UserID)
                                    };

                    //solutionLists = new List<SolutionList>();
                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    solutionList = new SolutionList();
                    //    solutionList.SOLUTION_ID = Convert.ToInt32(row["SOLUTION_ID"]);
                    //    solutionList.SOLUTION_NAME = row["SOLUTION_NAME"].ToString();
                    //    if (Convert.ToInt32(row["SOLUTION_ID"]) > 0)
                    //    {
                    //        solutionList.Packages = GetPackageList(Convert.ToInt32(row["SOLUTION_ID"]), UserID);
                    //    }

                                    //    solutionLists.Add(solutionList);
                                    //}
                }
            }
            mdt = DatabaseSettings.GetTransObject(solutionLists, StatusValue, "Record Found", dt);
            return mdt;
        }

        //private List<PackageList> GetPackageList(int SolutionID, int UserID)
        //{
        //    List<PackageList> packageLists = null;
        //    PackageList packageList = null;
        //    List<SqlParameter> prm = new List<SqlParameter>();
        //    SqlParameter Solution_ID = new SqlParameter("@SolutionID", SolutionID);
        //    prm.Add(Solution_ID);
        //    SqlParameter Status = new SqlParameter("@Status", 0);
        //    Status.Direction = ParameterDirection.Output;
        //    prm.Add(Status);
        //    int StatusValue = 0;
        //    DataTable dt = DatabaseSettings.GetDataSet("sp_GetPackageList", out StatusValue, prm).Tables[0];

        //    if (StatusValue == 1)
        //    {
        //        if (dt.Rows.Count > 0)
        //        {
        //            packageLists = new List<PackageList>();
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                packageList = new PackageList();
        //                packageList.PACKAGE_ID = Convert.ToInt32(row["PACKAGE_ID"]);
        //                packageList.PACKAGE_NAME = row["PACKAGE_NAME"].ToString();
        //                packageList.SOLUTION_ID = Convert.ToInt32(row["SOLUTION_ID"]);
        //                if (Convert.ToInt32(row["PACKAGE_ID"]) > 0)
        //                {
        //                    packageList.Configurations = GetConfigurationList(UserID, Convert.ToInt32(row["PACKAGE_ID"]));
        //                }

        //                packageLists.Add(packageList);
        //            }
        //        }
        //    }

        //    return packageLists;
        //}
        /// <summary>
        /// It return all package list for respective Solution
        /// </summary>
        /// <param name="SolutionID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private IEnumerable<PackageList> GetPackageList(int SolutionID, int UserID)
        {
            IEnumerable<PackageList> packageLists = null;
            PackageList packageList = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter Solution_ID = new SqlParameter("@SolutionID", SolutionID);
            prm.Add(Solution_ID);
            SqlParameter Status = new SqlParameter("@Status", 0);
            Status.Direction = ParameterDirection.Output;
            prm.Add(Status);
            int StatusValue = 0;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetPackageList", out StatusValue, prm).Tables[0];

            if (StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {
                    packageLists = from d in dt.AsEnumerable()
                                   select new PackageList
                                   {
                                       PACKAGE_ID = d.Field<int>("PACKAGE_ID"),
                                       PACKAGE_NAME = d.Field<string>("PACKAGE_NAME"),
                                       SOLUTION_ID = d.Field<int>("SOLUTION_ID"),
                                       Configurations = GetConfigurationList(UserID, d.Field<int>("PACKAGE_ID"))
                                   };

                }

                //packageLists = new List<PackageList>();
                //foreach (DataRow row in dt.Rows)
                //{
                //    packageList = new PackageList();
                //    packageList.PACKAGE_ID = Convert.ToInt32(row["PACKAGE_ID"]);
                //    packageList.PACKAGE_NAME = row["PACKAGE_NAME"].ToString();
                //    packageList.SOLUTION_ID = Convert.ToInt32(row["SOLUTION_ID"]);
                //    if (Convert.ToInt32(row["PACKAGE_ID"]) > 0)
                //    {
                //        packageList.Configurations = GetConfigurationList(UserID, Convert.ToInt32(row["PACKAGE_ID"]));
                //    }

                //    packageLists.Add(packageList);
                //}
            }
            return packageLists;
        }
        /// <summary>
        /// It will return Top 3 configurations for respective package and User
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        private IEnumerable<ConfigurationList> GetConfigurationList(int UserID, int PackageID)
        {
            IEnumerable<ConfigurationList> configLists = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter userID = new SqlParameter("@UserID", UserID);
            prm.Add(userID);
            SqlParameter packageID = new SqlParameter("@PackageID", PackageID);
            prm.Add(packageID);
            SqlParameter NoofConfiguration = new SqlParameter("@NoofConfiguration", 3);
            prm.Add(NoofConfiguration);

            SqlParameter Status = new SqlParameter("@Status", 0);
            Status.Direction = ParameterDirection.Output;
            prm.Add(Status);
            int StatusValue = 0;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetLatestConfiguration", out StatusValue, prm).Tables[0];

            if (StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {
                    configLists = from d in dt.AsEnumerable()
                                  select new ConfigurationList
                                  {
                                      CONFIGURATION_ID = d.Field<int>("CONFIGURATION_ID"),
                                      CONFIGURATION_NAME = d.Field<string>("CONFIGURATION_NAME"),
                                      PACKAGE_ID = d.Field<int>("PACKAGE_ID")
                                  };
                }
            }
            return configLists;
        }
    }
}
