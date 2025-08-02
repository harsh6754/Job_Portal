using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Model;
public class t_Job_Report
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_report_id{get;set;}

    public t_user? user{get;set;}
    public int c_user_id{get;set;}

    public Job_Post? job_Post{get;set;}
    public int c_job_id{get;set;}

    public t_Company? company{get;set;}
    public int c_company_id{get;set;}

    public string c_report_topic{get;set;}

    public string c_report_desc{get;set;}
}