using System;
using System.Data;
using System.Web;
namespace IKDWebApp.Ashx
{
    /// <summary>
    /// AjaxHandler 的摘要说明
    /// </summary>
    public class AjaxHandler : IHttpHandler
	{
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
		{
			context.Response.Expires = 0;
			context.Response.ContentType = "text/plain";
			context.Response.Charset = "utf-8";
			switch (context.Request["type"].ToString())
			{
                case "GetInsider":
                    GetInsider(context);
                    break;
                case "SaveOutsiderInfo":
                    SaveOutsiderInfo(context);
                    break;
                default:
					break;
			}
		}
        
        /// <summary>
        /// 访客自助申请
        /// </summary>
        /// <param name="context"></param>
        //////////////////////////////////////////访客///////////////////////////////////////////////

        //获取公司内员工的信息
        public void GetInsider(HttpContext context)
        {
            //从SQL中获取可用者的id，姓名和手机号
            string sql = "select id,lastname,mobile from hrmresource where loginid is not null and loginid != ''";
            DataTable dt = SqlClient.OASqlHelper.ExecuteQuery(sql).Tables[0];

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //将名字的第二个字改为*号
                    string min = dr["lastname"].ToString();
                    string starmin;
                    starmin = min.Remove(1, 1);
                    starmin = starmin.Insert(1, "*");

                    //仅保留末四位手机号
                    //若无手机号则留空
                    string phone = dr["mobile"].ToString();
                    string LastFourPhone; 
                    if (phone.Length == 11)
                    {
                        LastFourPhone = phone.Substring(phone.Length - 4);
                    }
                    else
                    {
                        LastFourPhone = "";
                    }
                    
                    //将姓名与手机组合放select中
                    context.Response.Write("<option value=\"" + dr["id"].ToString() + "\">" + min + " " + LastFourPhone + "</option>\n");
                }

            }
        }

        //上传访客所填的身份信息
        public void SaveOutsiderInfo(HttpContext context)
        {
            //从html表单中获取姓名，电话，联系人信息，备注，以及当前时间
            string name = context.Request["name"].ToString();
            string outsiderID = context.Request["outsiderID"].ToString();
            string phone = context.Request["phone"].ToString();
            string InsiderID = context.Request["InsiderID"].ToString();
            string ousiderType = context.Request["ousiderType"].ToString();
            string numberofPerson = context.Request["numberofPerson"].ToString();
            string company = context.Request["company"].ToString();
            string desc = context.Request["desc"].ToString();
            string lTimeNow = context.Request["time"].ToString();

            //发起OA访客申请流程
            try
            {
                IKDVisiterVerify.WorkflowService.WorkflowServicePortTypeClient workflow1 = new IKDVisiterVerify.WorkflowService.WorkflowServicePortTypeClient();
                IKDVisiterVerify.WorkflowService.WorkflowRequestInfo workflowRequestInfo = new IKDVisiterVerify.WorkflowService.WorkflowRequestInfo();
                IKDVisiterVerify.WorkflowService.WorkflowRequestInfo requestInfo1 = new IKDVisiterVerify.WorkflowService.WorkflowRequestInfo();
                IKDVisiterVerify.WorkflowService.WorkflowBaseInfo workflowBaseInfo = new IKDVisiterVerify.WorkflowService.WorkflowBaseInfo();
               

                //填入访客流程BaseInfo及RequestInfo
                workflowBaseInfo.workflowId = "215";
                workflowBaseInfo.workflowName = "访客自助申请流程";
                workflowBaseInfo.workflowTypeName = "访客自助申请流程";
                workflowRequestInfo.workflowBaseInfo = workflowBaseInfo;
                workflowRequestInfo.requestName = "访客自助申请表";
                workflowRequestInfo.creatorId = "7620";
                workflowRequestInfo.canEdit = false;
                workflowRequestInfo.canView = true;
                workflowRequestInfo.mustInputRemark = false;
                workflowRequestInfo.needAffirmance = false;

                //创建主表单
                IKDVisiterVerify.WorkflowService.WorkflowMainTableInfo mainTableInfo = new IKDVisiterVerify.WorkflowService.WorkflowMainTableInfo();
                IKDVisiterVerify.WorkflowService.WorkflowRequestTableRecord[] requestTableRecords = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableRecord[1];
                IKDVisiterVerify.WorkflowService.WorkflowRequestTableField[] requestTableFields = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField[9];

                //向主表单填入信息
                requestTableFields[0] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访人姓名
                    fieldName = "lfrxm",
                    fieldValue = name,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[1] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访人手机
                    fieldName = "lfrsjh",
                    fieldValue = phone,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[2] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //联系人
                    fieldName = "lxr",
                    fieldValue = InsiderID,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[3] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访事由
                    fieldName = "bz",
                    fieldValue = desc,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[4] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //申请时间
                    fieldName = "sqrq",
                    fieldValue = lTimeNow,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                //requestTableFields[5] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                //{
                //    //申请人
                //    fieldName = "sqr",
                //    fieldValue = outsiderID,
                //    fieldOrder = 0,
                //    view = true,
                //    edit = true
                //};

                requestTableFields[6] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访对象
                    fieldName = "lfdx",
                    fieldValue = ousiderType,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[7] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访人数
                    fieldName = "lfrs",
                    fieldValue = numberofPerson,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                requestTableFields[8] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableField
                {
                    //来访单位
                    fieldName = "lfdw",
                    fieldValue = company,
                    fieldOrder = 0,
                    view = true,
                    edit = true
                };

                //主表单填写结束
                requestTableRecords[0] = new IKDVisiterVerify.WorkflowService.WorkflowRequestTableRecord();
                requestTableRecords[0].workflowRequestTableFields = requestTableFields;
                mainTableInfo.requestRecords = requestTableRecords;

                workflowRequestInfo.workflowMainTableInfo = mainTableInfo;

                //请求流程回执
                string respond = workflow1.doCreateWorkflowRequest(workflowRequestInfo, 7620);

                //向html返回请求的值
                context.Response.Write(respond);

            }
            catch(Exception ex)
            {
                //如果出错
                Console.WriteLine(ex.Message);

            }
        }

    }
}