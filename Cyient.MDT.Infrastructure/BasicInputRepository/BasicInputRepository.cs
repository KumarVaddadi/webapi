using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Cyient.MDT.WebAPI.Core.Entities.BasicInput;
using Cyient.MDT.WebAPI.Core.Interface.BasicInput;
using Cyient.MDT.WebAPI.Core.Common;
namespace Cyient.MDT.Infrastructure.BasicInputRepository
{
    public class BasicInputRepository : IBasicInputOptions
    {

        public BasicInputRepository() { }
        /// <summary>
        /// This will return list of Basic input options available in database
        /// </summary>
        /// <returns></returns>
        public MDTTransactionInfo GetBasicInputs()
        {
            MDTTransactionInfo mdt = new MDTTransactionInfo();
            List<BasicInput> basicInputs = null;
            BasicInput basicInput = null;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetBasicInput").Tables[0];
            if (dt.Rows.Count > 0)
            {
                basicInputs = new List<BasicInput>();
                foreach (DataRow row in dt.Rows)
                { 
                    basicInput = new BasicInput();
                    basicInput.BASIC_INPUT_ID = Convert.ToInt32(row["BASIC_INPUT_ID"]);
                    basicInput.BASIC_INPUT_NAME = row["BASIC_INPUT_NAME"].ToString();
                    if(Convert.ToInt32(row["BASIC_INPUT_ID"]) > 0)
                    {
                        basicInput.BasicInputOptions = GetBasicInputOptions(Convert.ToInt32(row["BASIC_INPUT_ID"]));
                    }

                    basicInputs.Add(basicInput);
                }
                if(basicInputs!=null)
                {
                    mdt.transactionObject = basicInputs;
                    mdt.status = System.Net.HttpStatusCode.OK;
                    mdt.msgCode = MessageCode.Success;
                    mdt.message = "Fetched basic input detail successfully";
                }
                else
                {
                    mdt.transactionObject = null;
                    mdt.status = System.Net.HttpStatusCode.NotFound;
                    mdt.msgCode = MessageCode.Failed;
                    mdt.message = "No record found";
                }



                ////For Setup Basic input
                //bs.Setup = from d in dt.AsEnumerable()
                //           where d.Field<string>("BASIC_NAME") == "Setup"
                //           select new BasicInput
                //           {
                //               BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //               INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //           };
                ////For Fuel Basic input
                //bs.Fuel = from d in dt.AsEnumerable()
                //          where d.Field<string>("BASIC_NAME") == "Fuel"
                //          select new BasicInput
                //          {
                //              BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //              INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //          };
                ////For Without Fuel Gas Skid Basic input
                //bs.WFGS = from d in dt.AsEnumerable()
                //          where d.Field<string>("BASIC_NAME") == "Without Fuel Gas Skid"
                //          select new BasicInput
                //          {
                //              BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //              INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //          };
                ////For Climate Basic input
                //bs.Climate = from d in dt.AsEnumerable()
                //             where d.Field<string>("BASIC_NAME") == "Climate"
                //             select new BasicInput
                //             {
                //                 BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //                 INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //             };
                ////For WHRU Basic input
                //bs.WHRU = from d in dt.AsEnumerable()
                //          where d.Field<string>("BASIC_NAME") == "WHRU"
                //          select new BasicInput
                //          {
                //              BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //              INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //          };
                ////For Location Basic input
                //bs.Location = from d in dt.AsEnumerable()
                //              where d.Field<string>("BASIC_NAME") == "Location"
                //              select new BasicInput
                //              {
                //                  BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //                  INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //              };
                ////For ATEX Basic input
                //bs.ATEX = from d in dt.AsEnumerable()
                //          where d.Field<string>("BASIC_NAME") == "ATEX"
                //          select new BasicInput
                //          {
                //              BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //              INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //          };
                ////For sound attenuation Basic input
                //bs.SoundAtt = from d in dt.AsEnumerable()
                //              where d.Field<string>("BASIC_NAME") == "sound attenuation"
                //              select new BasicInput
                //              {
                //                  BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //                  INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //              };

                ////For Grid Frequency Basic input
                //bs.GridFrequency = from d in dt.AsEnumerable()
                //              where d.Field<string>("BASIC_NAME") == "Grid Frequency"
                //                   select new BasicInput
                //              {
                //                  BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                //                  INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME")
                //              };
            }
            return mdt;
        }


        private IEnumerable<BasicInputOptions> GetBasicInputOptions(int BasicInputID)
        {
            IEnumerable<BasicInputOptions> basicInputOptions = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter Basic_Input_ID = new SqlParameter("@BASIC_INPUT_ID", BasicInputID);
            prm.Add(Basic_Input_ID);

            DataTable dt = DatabaseSettings.GetDataSet("sp_GetBasicInputDetails", prm).Tables[0];

            if (dt.Rows.Count > 0)
            {
                basicInputOptions = from d in dt.AsEnumerable()
                                 select new BasicInputOptions
                                 {
                                     BASIC_INPUT_OPTN_ID = d.Field<int>("BASIC_INPUT_OPTN_ID"),
                                     INPUT_OPT_NAME = d.Field<string>("INPUT_OPT_NAME"),
                                     BASIC_INPUT_ID = d.Field<int>("BASIC_INPUT_ID")
                                 };
            }

            return basicInputOptions;
        }
    }
}
