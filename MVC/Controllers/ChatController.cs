using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Models;

namespace MVC.Controllers
{

    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public JsonResult GetResponse(string message)
        {
            var response = GenerateBotResponse(message);
            return Json(new ChatMessage { UserMessage = message, BotResponse = response });
        }


        private string GenerateBotResponse(string msg)
        {
            // Greetings
            if (msg.IndexOf("hello", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("hi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("hey", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Hello! Welcome to Careerlink. How can I assist you today?";

            if (msg.IndexOf("नमस्ते", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("हाय", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("हे", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नमस्ते! Careerlink में आपका स्वागत है। मैं आपकी किस प्रकार सहायता कर सकता हूँ?";

            if (msg.IndexOf("હેલો", StringComparison.OrdinalIgnoreCase) >= 0 ||
    msg.IndexOf("હાય", StringComparison.OrdinalIgnoreCase) >= 0 ||
    msg.IndexOf("નમસ્તે", StringComparison.OrdinalIgnoreCase) >= 0)
                return "નમસ્તે! Careerlink માં આપનું સ્વાગત છે. હું તમારી કેવી રીતે સહાય કરી શકું?";

            if (msg.IndexOf("नमस्कार", StringComparison.OrdinalIgnoreCase) >= 0 ||
            msg.IndexOf("हाय", StringComparison.OrdinalIgnoreCase) >= 0 ||
            msg.IndexOf("हॅलो", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नमस्कार! Careerlink मध्ये तुमचं स्वागत आहे. मी तुम्हाला कशाप्रकारे मदत करू शकतो?";


            if (msg.IndexOf("good morning", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Good morning! How can I help you with your job search today?";

            if (msg.IndexOf("सुप्रभात", StringComparison.OrdinalIgnoreCase) >= 0)
                return "सुप्रभात! मैं आपकी नौकरी खोज में कैसे मदद कर सकता हूँ?";

            if (msg.IndexOf("શુભ સવાર", StringComparison.OrdinalIgnoreCase) >= 0)
                return "શુભ સવાર! હું તમારી નોકરી શોધવામાં કેવી રીતે મદદ કરી શકું?";

            if (msg.IndexOf("शुभ सकाळ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "शुभ सकाळ! मी तुमच्या नोकरी शोधण्यात कशी मदत करू शकतो?";

            if (msg.IndexOf("good evening", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Good evening! How can I support your career goals?";

            if (msg.IndexOf("शुभ संध्या", StringComparison.OrdinalIgnoreCase) >= 0)
                return "शुभ संध्या! मैं आपके करियर लक्ष्यों में आपकी कैसे मदद कर सकता हूँ?";

            if (msg.IndexOf("શુભ સાંજ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "શુભ સાંજ! હું તમારા કારકિર્દી લક્ષ્યોમાં કેવી રીતે મદદ કરી શકું?";

            if (msg.IndexOf("शुभ संध्याकाळ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "शुभ संध्याकाळ! मी तुमच्या करिअरच्या उद्दिष्टांमध्ये कशी मदत करू शकतो?";

            // Account Issues
            if (msg.IndexOf("login issue", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("can't login", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                return "If you're having trouble logging in, try resetting your password using the 'Forgot Password' link.";

            if (msg.IndexOf("लॉगिन समस्या", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("लॉगिन नहीं कर सकता", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("पासवर्ड", StringComparison.OrdinalIgnoreCase) >= 0)
                return "यदि आपको लॉगिन करने में समस्या हो रही है, तो 'पासवर्ड भूल गए' लिंक का उपयोग करके अपना पासवर्ड रीसेट करने का प्रयास करें।";

            if (msg.IndexOf("લૉગિન સમસ્યા", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("લૉગિન કરી શકતો નથી", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("પાસવર્ડ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "જો તમને લૉગિન કરવામાં સમસ્યા આવી રહી છે, તો 'પાસવર્ડ ભૂલી ગયા' લિંકનો ઉપયોગ કરીને તમારું પાસવર્ડ રીસેટ કરવાનો પ્રયાસ કરો.";

            if (msg.IndexOf("लॉगिन समस्या", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("लॉगिन करू शकत नाही", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("पासवर्ड", StringComparison.OrdinalIgnoreCase) >= 0)
                return "जर तुम्हाला लॉगिन करण्यात अडचण येत असेल, तर 'पासवर्ड विसरलात' लिंक वापरून तुमचा पासवर्ड रीसेट करण्याचा प्रयत्न करा.";

            if (msg.IndexOf("register", StringComparison.OrdinalIgnoreCase) >= 0 ||
    msg.IndexOf("sign up", StringComparison.OrdinalIgnoreCase) >= 0)
                return "To register, please visit the signup page and provide your basic details.";

            if (msg.IndexOf("रजिस्टर", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("साइन अप", StringComparison.OrdinalIgnoreCase) >= 0)
                return "रजिस्टर करने के लिए, कृपया साइनअप पेज पर जाएं और अपनी बुनियादी जानकारी प्रदान करें।";

            if (msg.IndexOf("રજિસ્ટર", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("સાઇન અપ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "રજિસ્ટર કરવા માટે, કૃપા કરીને સાઇનઅપ પેજ પર જાઓ અને તમારી મૂળભૂત વિગતો પૂરી પાડો.";

            if (msg.IndexOf("नोंदणी", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("साइन अप", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नोंदणी करण्यासाठी, कृपया साइनअप पृष्ठावर जा आणि आपली मूलभूत माहिती प्रदान करा.";

            if (msg.IndexOf("delete account", StringComparison.OrdinalIgnoreCase) >= 0)
                return "To delete your account, go to Account Settings > Privacy > Delete Account.";

            if (msg.IndexOf("खाता हटाएं", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("अकाउंट डिलीट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "अपना खाता हटाने के लिए, खाता सेटिंग्स > गोपनीयता > खाता हटाएं पर जाएं।";

            if (msg.IndexOf("ખાતું કાઢી નાખો", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("એકાઉન્ટ ડિલીટ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમારું ખાતું કાઢી નાખવા માટે, ખાતું સેટિંગ્સ > પ્રાઇવસી > ખાતું કાઢી નાખો પર જાઓ.";

            if (msg.IndexOf("खाते हटवा", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("अकाउंट डिलीट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुमचं खातं हटवण्यासाठी, खाते सेटिंग्ज > गोपनीयता > खाते हटवा येथे जा.";

            // Job Search
            if (msg.IndexOf("find job", StringComparison.OrdinalIgnoreCase) >= 0 ||
      msg.IndexOf("job search", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You can search for jobs by title, location, or category using our search bar.";

            if (msg.IndexOf("नौकरी खोजें", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("नौकरी खोज", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आप शीर्षक, स्थान, या श्रेणी के आधार पर हमारी खोज पट्टी का उपयोग करके नौकरियां खोज सकते हैं।";

            if (msg.IndexOf("નોકરી શોધો", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("નોકરી શોધ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમે શીર્ષક, સ્થાન, અથવા શ્રેણી દ્વારા અમારી શોધ બારનો ઉપયોગ કરીને નોકરીઓ શોધી શકો છો.";

            if (msg.IndexOf("नोकरी शोधा", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("नोकरी शोध", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुम्ही शीर्षक, स्थान, किंवा श्रेणीद्वारे आमच्या शोध पट्टीचा वापर करून नोकऱ्या शोधू शकता.";

            if (msg.IndexOf("remote jobs", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Use the 'Remote' filter on the job listings page to find remote opportunities.";

            if (msg.IndexOf("दूरस्थ नौकरियां", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("वर्क फ्रॉम होम", StringComparison.OrdinalIgnoreCase) >= 0)
                return "दूरस्थ अवसर खोजने के लिए नौकरी सूची पृष्ठ पर 'Remote' फ़िल्टर का उपयोग करें।";

            if (msg.IndexOf("રિમોટ નોકરીઓ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("ઘરેથી કામ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "રિમોટ તકો શોધવા માટે નોકરી લિસ્ટિંગ પેજ પર 'Remote' ફિલ્ટરનો ઉપયોગ કરો.";

            if (msg.IndexOf("रिमोट नोकऱ्या", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("घरून काम", StringComparison.OrdinalIgnoreCase) >= 0)
                return "रिमोट संधी शोधण्यासाठी नोकरी सूची पृष्ठावरील 'Remote' फिल्टर वापरा.";

            if (msg.IndexOf("internship", StringComparison.OrdinalIgnoreCase) >= 0)
                return "We have a dedicated Internship section. Filter by 'Internship' in job type.";

            if (msg.IndexOf("इंटर्नशिप", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हमारे पास एक समर्पित इंटर्नशिप अनुभाग है। नौकरी प्रकार में 'इंटर्नशिप' फ़िल्टर का उपयोग करें।";

            if (msg.IndexOf("ઇન્ટર્નશિપ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમારા પાસે એક સમર્પિત ઇન્ટર્નશિપ વિભાગ છે. નોકરીના પ્રકારમાં 'ઇન્ટર્નશિપ' ફિલ્ટરનો ઉપયોગ કરો.";

            if (msg.IndexOf("इंटर्नशिप", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आमच्याकडे एक समर्पित इंटर्नशिप विभाग आहे. नोकरी प्रकारात 'इंटर्नशिप' फिल्टर वापरा.";


            // Resume Help
            if (msg.IndexOf("upload resume", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You can upload your resume from your dashboard. Accepted formats: PDF, DOCX.";

            if (msg.IndexOf("अपना रिज्यूमे अपलोड करें", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("रिज्यूमे अपलोड", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आप अपने डैशबोर्ड से अपना रिज्यूमे अपलोड कर सकते हैं। स्वीकृत प्रारूप: PDF, DOCX।";

            if (msg.IndexOf("તમારું રિઝ્યુમ અપલોડ કરો", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("રિઝ્યુમ અપલોડ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમે તમારા ડેશબોર્ડ પરથી તમારું રિઝ્યુમ અપલોડ કરી શકો છો. સ્વીકાર્ય ફોર્મેટ્સ: PDF, DOCX.";

            if (msg.IndexOf("तुमचं रिझ्युमे अपलोड करा", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("रिझ्युमे अपलोड", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुम्ही तुमच्या डॅशबोर्डवरून तुमचं रिझ्युमे अपलोड करू शकता. स्वीकारलेले स्वरूप: PDF, DOCX.";

            if (msg.IndexOf("resume builder", StringComparison.OrdinalIgnoreCase) >= 0)
                return "We offer a built-in Resume Builder under 'Tools' > 'Resume Builder'.";

            if (msg.IndexOf("रिज्यूमे बिल्डर", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हम 'टूल्स' > 'रिज्यूमे बिल्डर' के तहत एक इनबिल्ट रिज्यूमे बिल्डर प्रदान करते हैं।";

            if (msg.IndexOf("રિઝ્યુમ બિલ્ડર", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમે 'ટૂલ્સ' > 'રિઝ્યુમ બિલ્ડર' હેઠળ એક ઇનબિલ્ટ રિઝ્યુમ બિલ્ડર પ્રદાન કરીએ છીએ.";

            if (msg.IndexOf("रिझ्युमे बिल्डर", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आम्ही 'टूल्स' > 'रिझ्युमे बिल्डर' अंतर्गत एक इनबिल्ट रिझ्युमे बिल्डर प्रदान करतो.";

            if (msg.IndexOf("resume tips", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Check out our blog for tips on creating an effective resume.";

            if (msg.IndexOf("रिज्यूमे टिप्स", StringComparison.OrdinalIgnoreCase) >= 0)
                return "एक प्रभावी रिज्यूमे बनाने के लिए सुझावों के लिए हमारे ब्लॉग को देखें।";

            if (msg.IndexOf("રિઝ્યુમ ટીપ્સ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "પ્રભાવશાળી રિઝ્યુમ બનાવવા માટેના ટીપ્સ માટે અમારા બ્લોગને જુઓ.";

            if (msg.IndexOf("रिझ्युमे टिप्स", StringComparison.OrdinalIgnoreCase) >= 0)
                return "प्रभावी रिझ्युमे तयार करण्यासाठी टिप्ससाठी आमच्या ब्लॉगला भेट द्या.";

            // Application Help
            if (msg.IndexOf("how to apply", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Click on any job and use the 'Apply Now' button to submit your application.";

            if (msg.IndexOf("कैसे आवेदन करें", StringComparison.OrdinalIgnoreCase) >= 0)
                return "किसी भी नौकरी पर क्लिक करें और अपना आवेदन जमा करने के लिए 'Apply Now' बटन का उपयोग करें।";

            if (msg.IndexOf("કેવી રીતે અરજી કરવી", StringComparison.OrdinalIgnoreCase) >= 0)
                return "કોઈપણ નોકરી પર ક્લિક કરો અને તમારું અરજી સબમિટ કરવા માટે 'Apply Now' બટનનો ઉપયોગ કરો.";

            if (msg.IndexOf("अर्ज कसा करायचा", StringComparison.OrdinalIgnoreCase) >= 0)
                return "कोणत्याही नोकरीवर क्लिक करा आणि तुमचा अर्ज सबमिट करण्यासाठी 'Apply Now' बटण वापरा.";

            if (msg.IndexOf("application status", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You can check your application status in the 'My Applications' section.";

            if (msg.IndexOf("आवेदन की स्थिति", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आप 'My Applications' अनुभाग में अपनी आवेदन स्थिति की जांच कर सकते हैं।";

            if (msg.IndexOf("અરજીની સ્થિતિ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમે 'My Applications' વિભાગમાં તમારી અરજીની સ્થિતિ તપાસી શકો છો.";

            if (msg.IndexOf("अर्जाची स्थिती", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुम्ही 'My Applications' विभागात तुमच्या अर्जाची स्थिती तपासू शकता.";

            if (msg.IndexOf("rejected", StringComparison.OrdinalIgnoreCase) >= 0)
                return "If you’ve been rejected, don't be discouraged. Keep applying and updating your resume.";

            if (msg.IndexOf("अस्वीकृत", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("रिजेक्ट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "यदि आपको अस्वीकृत कर दिया गया है, तो निराश न हों। आवेदन करते रहें और अपना रिज्यूमे अपडेट करते रहें।";

            if (msg.IndexOf("નકારવામાં આવ્યું", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("રિજેક્ટ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "જો તમને નકારવામાં આવ્યું છે, તો નિરાશ ન થાઓ. અરજી કરવાનું ચાલુ રાખો અને તમારું રિઝ્યુમ અપડેટ કરો.";

            if (msg.IndexOf("नाकारले", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("रिजेक्ट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "जर तुम्हाला नाकारले गेले असेल, तर निराश होऊ नका. अर्ज करत राहा आणि तुमचे रिझ्युमे अपडेट करत राहा.";

            // Interview Prep
            if (msg.IndexOf("interview tips", StringComparison.OrdinalIgnoreCase) >= 0)
                return "We offer curated interview tips in our 'Career Advice' section.";

            if (msg.IndexOf("इंटरव्यू टिप्स", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हमारे 'Career Advice' अनुभाग में क्यूरेटेड इंटरव्यू टिप्स उपलब्ध हैं।";

            if (msg.IndexOf("ઇન્ટરવ્યુ ટીપ્સ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમારા 'Career Advice' વિભાગમાં ક્યુરેટેડ ઇન્ટરવ્યુ ટીપ્સ ઉપલબ્ધ છે.";

            if (msg.IndexOf("इंटरव्ह्यू टिप्स", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आमच्या 'Career Advice' विभागात क्युरेटेड इंटरव्ह्यू टिप्स उपलब्ध आहेत.";

            if (msg.IndexOf("mock interview", StringComparison.OrdinalIgnoreCase) >= 0)
                return "We’re working on launching a Mock Interview module soon.";

            if (msg.IndexOf("मॉक इंटरव्यू", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हम जल्द ही एक मॉक इंटरव्यू मॉड्यूल लॉन्च करने पर काम कर रहे हैं।";

            if (msg.IndexOf("મોક ઇન્ટરવ્યુ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમે ટૂંક સમયમાં મોક ઇન્ટરવ્યુ મોડ્યુલ લોન્ચ કરવા પર કામ કરી રહ્યા છીએ.";

            if (msg.IndexOf("मॉक इंटरव्ह्यू", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आम्ही लवकरच मॉक इंटरव्ह्यू मॉड्यूल लॉन्च करण्यावर काम करत आहोत.";

            if (msg.IndexOf("what to wear", StringComparison.OrdinalIgnoreCase) >= 0)
                return "For virtual interviews, wear smart formals. Dress to impress, even at home.";

            if (msg.IndexOf("क्या पहनना चाहिए", StringComparison.OrdinalIgnoreCase) >= 0)
                return "वर्चुअल इंटरव्यू के लिए स्मार्ट फॉर्मल्स पहनें। घर पर भी प्रभाव डालने के लिए तैयार रहें।";

            if (msg.IndexOf("શું પહેરવું", StringComparison.OrdinalIgnoreCase) >= 0)
                return "વર્ચ્યુઅલ ઇન્ટરવ્યુ માટે સ્માર્ટ ફોર્મલ્સ પહેરો. ઘરમાં પણ પ્રભાવ પાડવા માટે તૈયાર રહો.";

            if (msg.IndexOf("काय घालावे", StringComparison.OrdinalIgnoreCase) >= 0)
                return "व्हर्च्युअल मुलाखतीसाठी स्मार्ट फॉर्मल्स घाला. घरी असतानाही प्रभाव पाडण्यासाठी तयार राहा.";

            // Career Guidance
            if (msg.IndexOf("career advice", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Our Career Advice section offers articles on resume writing, interviews, and more.";

            if (msg.IndexOf("करियर सलाह", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हमारा करियर सलाह अनुभाग रिज्यूमे लेखन, इंटरव्यू और अधिक पर लेख प्रदान करता है।";

            if (msg.IndexOf("કારકિર્દી સલાહ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમારું કારકિર્દી સલાહ વિભાગ રિઝ્યુમ લખાણ, ઇન્ટરવ્યુ અને વધુ પર લેખો પ્રદાન કરે છે.";

            if (msg.IndexOf("करिअर सल्ला", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आमचा करिअर सल्ला विभाग रिझ्युमे लेखन, मुलाखती आणि बरेच काही यावर लेख प्रदान करतो.";

            if (msg.IndexOf("which career", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Take our Career Assessment quiz to discover the best roles for your profile.";

            if (msg.IndexOf("कौन सा करियर", StringComparison.OrdinalIgnoreCase) >= 0)
                return "अपने प्रोफाइल के लिए सबसे अच्छे रोल खोजने के लिए हमारा करियर असेसमेंट क्विज़ लें।";

            if (msg.IndexOf("કયું કારકિર્દી", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમારા પ્રોફાઇલ માટે શ્રેષ્ઠ ભૂમિકા શોધવા માટે અમારું કારકિર્દી મૂલ્યાંકન ક્વિઝ લો.";

            if (msg.IndexOf("कोणता करिअर", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुमच्या प्रोफाइलसाठी सर्वोत्तम भूमिका शोधण्यासाठी आमचा करिअर असेसमेंट क्विझ घ्या.";

            if (msg.IndexOf("job switch", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Switching careers? Try browsing different job categories and update your resume accordingly.";

            if (msg.IndexOf("करियर बदलें", StringComparison.OrdinalIgnoreCase) >= 0)
                return "करियर बदल रहे हैं? विभिन्न नौकरी श्रेणियों को ब्राउज़ करें और तदनुसार अपना रिज्यूमे अपडेट करें।";

            if (msg.IndexOf("કારકિર્દી બદલો", StringComparison.OrdinalIgnoreCase) >= 0)
                return "કારકિર્દી બદલી રહ્યા છો? વિવિધ નોકરી શ્રેણીઓ બ્રાઉઝ કરો અને તમારું રિઝ્યુમ તદનુસાર અપડેટ કરો.";

            if (msg.IndexOf("करिअर बदला", StringComparison.OrdinalIgnoreCase) >= 0)
                return "करिअर बदलत आहात? विविध नोकरी श्रेण्या ब्राउझ करा आणि त्यानुसार तुमचं रिझ्युमे अपडेट करा.";

            // Notifications & Alerts
            // Notifications & Alerts
            if (msg.IndexOf("job alerts", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Set job alerts from your dashboard to get notified of new openings.";

            if (msg.IndexOf("नौकरी अलर्ट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नई नौकरियों की सूचना पाने के लिए अपने डैशबोर्ड से नौकरी अलर्ट सेट करें।";

            if (msg.IndexOf("નોકરી એલર્ટ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "નવી નોકરીઓની સૂચના મેળવવા માટે તમારા ડેશબોર્ડમાંથી નોકરી એલર્ટ સેટ કરો.";

            if (msg.IndexOf("नोकरी अलर्ट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नवीन नोकऱ्यांची सूचना मिळवण्यासाठी तुमच्या डॅशबोर्डवरून नोकरी अलर्ट सेट करा.";

            if (msg.IndexOf("email notification", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You can manage your email preferences from your account settings.";

            if (msg.IndexOf("ईमेल अधिसूचना", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आप अपनी खाता सेटिंग्स से अपनी ईमेल प्राथमिकताएँ प्रबंधित कर सकते हैं।";

            if (msg.IndexOf("ઇમેઇલ સૂચના", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમે તમારા ખાતા સેટિંગ્સમાંથી તમારી ઇમેઇલ પ્રાથમિકતાઓ મેનેજ કરી શકો છો.";

            if (msg.IndexOf("ईमेल सूचना", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुम्ही तुमच्या खात्याच्या सेटिंग्जमधून तुमच्या ईमेल प्राधान्ये व्यवस्थापित करू शकता.";

            // Company Reviews
            if (msg.IndexOf("company reviews", StringComparison.OrdinalIgnoreCase) >= 0)
                return "We provide company reviews and ratings. Click on a company to read employee feedback.";

            if (msg.IndexOf("कंपनी समीक्षा", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हम कंपनी की समीक्षाएँ और रेटिंग प्रदान करते हैं। कर्मचारी फीडबैक पढ़ने के लिए किसी कंपनी पर क्लिक करें।";

            if (msg.IndexOf("કંપની સમીક્ષાઓ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમે કંપની સમીક્ષાઓ અને રેટિંગ્સ પ્રદાન કરીએ છીએ. કર્મચારી પ્રતિસાદ વાંચવા માટે કંપની પર ક્લિક કરો.";

            if (msg.IndexOf("कंपनी पुनरावलोकने", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आम्ही कंपनी पुनरावलोकने आणि रेटिंग प्रदान करतो. कर्मचारी अभिप्राय वाचण्यासाठी कंपनीवर क्लिक करा.";

            // Salary & Compensation
            if (msg.IndexOf("salary", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Check our Salary Insights page for average salaries by role and location.";

            if (msg.IndexOf("वेतन", StringComparison.OrdinalIgnoreCase) >= 0)
                return "भूमिका और स्थान के अनुसार औसत वेतन के लिए हमारे वेतन अंतर्दृष्टि पृष्ठ की जाँच करें।";

            if (msg.IndexOf("પગાર", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ભૂમિકા અને સ્થાન અનુસાર સરેરાશ પગાર માટે અમારા પગાર ઇન્સાઇટ્સ પેજની તપાસ કરો.";

            if (msg.IndexOf("पगार", StringComparison.OrdinalIgnoreCase) >= 0)
                return "भूमिका आणि स्थानानुसार सरासरी पगारासाठी आमच्या पगार अंतर्दृष्टी पृष्ठाची तपासणी करा.";

            // Technical Help
            if (msg.IndexOf("site not working", StringComparison.OrdinalIgnoreCase) >= 0 ||
    msg.IndexOf("bug", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Try clearing your browser cache. If issues persist, email support@careerlink.com.";

            if (msg.IndexOf("साइट काम नहीं कर रही है", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("बग", StringComparison.OrdinalIgnoreCase) >= 0)
                return "अपना ब्राउज़र कैश साफ़ करने का प्रयास करें। यदि समस्या बनी रहती है, तो support@careerlink.com पर ईमेल करें।";

            if (msg.IndexOf("સાઇટ કામ કરી રહી નથી", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("બગ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમારા બ્રાઉઝર કેશને સાફ કરવાનો પ્રયાસ કરો. જો સમસ્યા ચાલુ રહે, તો support@careerlink.com પર ઇમેઇલ કરો.";

            if (msg.IndexOf("साइट चालू नाही", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("बग", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुमच्या ब्राउझर कॅशे साफ करण्याचा प्रयत्न करा. जर समस्या कायम राहिली, तर support@careerlink.com वर ईमेल करा.";

            if (msg.IndexOf("upload error", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Ensure your file size is below the limit and the format is supported.";

            if (msg.IndexOf("अपलोड त्रुटि", StringComparison.OrdinalIgnoreCase) >= 0)
                return "सुनिश्चित करें कि आपकी फ़ाइल का आकार सीमा के भीतर है और प्रारूप समर्थित है।";

            if (msg.IndexOf("અપલોડ ભૂલ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ખાતરી કરો કે તમારી ફાઇલનું કદ મર્યાદા હેઠળ છે અને ફોર્મેટ સપોર્ટેડ છે.";

            if (msg.IndexOf("अपलोड त्रुटी", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुमची फाइल आकार मर्यादेत आहे आणि स्वरूप समर्थित आहे याची खात्री करा.";

            // Subscription / Plans
            if (msg.IndexOf("premium", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Premium members get resume reviews, priority listings, and exclusive jobs. Visit the Premium page to upgrade.";

            if (msg.IndexOf("प्रीमियम", StringComparison.OrdinalIgnoreCase) >= 0)
                return "प्रीमियम सदस्य रिज्यूमे समीक्षा, प्राथमिकता सूचीकरण, और विशेष नौकरियां प्राप्त करते हैं। अपग्रेड करने के लिए प्रीमियम पेज पर जाएं।";

            if (msg.IndexOf("પ્રીમિયમ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "પ્રીમિયમ સભ્યોને રિઝ્યુમ સમીક્ષા, પ્રાથમિકતા સૂચિ, અને વિશિષ્ટ નોકરીઓ મળે છે. અપગ્રેડ કરવા માટે પ્રીમિયમ પેજ પર જાઓ.";

            if (msg.IndexOf("प्रीमियम", StringComparison.OrdinalIgnoreCase) >= 0)
                return "प्रीमियम सदस्यांना रिझ्युमे पुनरावलोकने, प्राधान्य यादी, आणि विशेष नोकऱ्या मिळतात. अपग्रेड करण्यासाठी प्रीमियम पृष्ठाला भेट द्या.";

            if (msg.IndexOf("free plan", StringComparison.OrdinalIgnoreCase) >= 0)
                return "The Free plan allows unlimited job search and 10 applications per month.";

            if (msg.IndexOf("फ्री प्लान", StringComparison.OrdinalIgnoreCase) >= 0)
                return "फ्री प्लान असीमित नौकरी खोज और प्रति माह 10 आवेदन की अनुमति देता है।";

            if (msg.IndexOf("ફ્રી પ્લાન", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ફ્રી પ્લાન અમર્યાદિત નોકરી શોધ અને દર મહિને 10 અરજીઓની મંજૂરી આપે છે.";

            if (msg.IndexOf("फ्री प्लॅन", StringComparison.OrdinalIgnoreCase) >= 0)
                return "फ्री प्लॅन अमर्यादित नोकरी शोध आणि दर महिन्याला 10 अर्ज करण्याची परवानगी देतो.";

            // General Queries
            if (msg.IndexOf("contact support", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You can reach our support team at support@careerlink.com or call 1800-CAREER.";

            if (msg.IndexOf("संपर्क सहायता", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आप हमारी सहायता टीम से support@careerlink.com पर संपर्क कर सकते हैं या 1800-CAREER पर कॉल कर सकते हैं।";

            if (msg.IndexOf("સંપર્ક સપોર્ટ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમે અમારી સપોર્ટ ટીમ support@careerlink.com પર સંપર્ક કરી શકો છો અથવા 1800-CAREER પર કૉલ કરી શકો છો.";

            if (msg.IndexOf("संपर्क समर्थन", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुम्ही आमच्या समर्थन टीमशी support@careerlink.com वर संपर्क साधू शकता किंवा 1800-CAREER वर कॉल करू शकता.";

            if (msg.IndexOf("working hours", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Our support team is available Mon–Fri, 9am to 6pm IST.";

            if (msg.IndexOf("कार्य समय", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हमारी सहायता टीम सोमवार से शुक्रवार, सुबह 9 बजे से शाम 6 बजे तक उपलब्ध है।";

            if (msg.IndexOf("કાર્ય સમય", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમારી સપોર્ટ ટીમ સોમવારથી શુક્રવાર, સવારે 9 વાગ્યાથી સાંજે 6 વાગ્યા સુધી ઉપલબ્ધ છે.";

            if (msg.IndexOf("कामाचे तास", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आमची समर्थन टीम सोमवार ते शुक्रवार, सकाळी 9 ते संध्याकाळी 6 पर्यंत उपलब्ध आहे.";

            if (msg.IndexOf("faq", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Visit our FAQ page for answers to common questions.";

            if (msg.IndexOf("सामान्य प्रश्न", StringComparison.OrdinalIgnoreCase) >= 0)
                return "सामान्य प्रश्नों के उत्तरों के लिए हमारे FAQ पृष्ठ पर जाएं।";

            if (msg.IndexOf("વારંવાર પૂછાતા પ્રશ્નો", StringComparison.OrdinalIgnoreCase) >= 0)
                return "વારંવાર પૂછાતા પ્રશ્નોના જવાબ માટે અમારા FAQ પેજ પર જાઓ.";

            if (msg.IndexOf("सामान्य प्रश्न", StringComparison.OrdinalIgnoreCase) >= 0)
                return "सामान्य प्रश्नांची उत्तरे मिळवण्यासाठी आमच्या FAQ पृष्ठाला भेट द्या.";

            // Bot Info
            if (msg.IndexOf("tell me about yourself", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I am a virtual assistant here to help you with your job search and career-related queries.";

            if (msg.IndexOf("अपने बारे में बताओ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मैं एक वर्चुअल सहायक हूँ, जो आपकी नौकरी खोज और करियर से संबंधित प्रश्नों में मदद करने के लिए यहाँ हूँ।";

            if (msg.IndexOf("તમારા વિશે કહો", StringComparison.OrdinalIgnoreCase) >= 0)
                return "હું એક વર્ચ્યુઅલ સહાયક છું, જે તમારી નોકરી શોધ અને કારકિર્દી સંબંધિત પ્રશ્નોમાં મદદ કરવા માટે અહીં છું.";

            if (msg.IndexOf("तुझ्याबद्दल सांग", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मी एक वर्च्युअल सहाय्यक आहे, जो तुमच्या नोकरी शोध आणि करिअर संबंधित प्रश्नांमध्ये मदत करण्यासाठी येथे आहे.";

            if (msg.IndexOf("what is your name", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I am Careerlink Bot, your virtual assistant for job search and career advice.";

            if (msg.IndexOf("आपका नाम क्या है", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मैं Careerlink Bot हूँ, आपकी नौकरी खोज और करियर सलाह के लिए आपका वर्चुअल सहायक।";

            if (msg.IndexOf("તમારું નામ શું છે", StringComparison.OrdinalIgnoreCase) >= 0)
                return "હું Careerlink Bot છું, તમારી નોકરી શોધ અને કારકિર્દી સલાહ માટે તમારું વર્ચ્યુઅલ સહાયક.";

            if (msg.IndexOf("तुझं नाव काय आहे", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मी Careerlink Bot आहे, तुमच्या नोकरी शोध आणि करिअर सल्ल्यासाठी तुमचा वर्च्युअल सहाय्यक.";

            if (msg.IndexOf("what can you do", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I can assist you with job searches, resume building, interview tips, and more. Just ask!";

            if (msg.IndexOf("आप क्या कर सकते हैं", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मैं आपको नौकरी खोज, रिज्यूमे बनाने, इंटरव्यू टिप्स और बहुत कुछ में मदद कर सकता हूँ। बस पूछें!";

            if (msg.IndexOf("તમે શું કરી શકો છો", StringComparison.OrdinalIgnoreCase) >= 0)
                return "હું તમને નોકરી શોધ, રિઝ્યુમ બનાવવામાં, ઇન્ટરવ્યુ ટીપ્સ અને વધુમાં મદદ કરી શકું છું. ફક્ત પૂછો!";

            if (msg.IndexOf("तू काय करू शकतोस", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("तू काय करू शकतेस", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मी तुम्हाला नोकरी शोध, रिझ्युमे तयार करणे, मुलाखत टिप्स आणि बरेच काही करण्यात मदत करू शकतो. फक्त विचारा!";

            if (msg.IndexOf("what is careerlink", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink is a job search platform that connects job seekers with employers. We offer various tools to help you in your job search.";

            if (msg.IndexOf("Careerlink क्या है", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink एक नौकरी खोज प्लेटफ़ॉर्म है जो नौकरी चाहने वालों को नियोक्ताओं से जोड़ता है। हम आपकी नौकरी खोज में मदद करने के लिए विभिन्न उपकरण प्रदान करते हैं।";

            if (msg.IndexOf("Careerlink શું છે", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink એ નોકરી શોધવા માટેનું પ્લેટફોર્મ છે જે નોકરી શોધનારાઓને નોકરીદાતાઓ સાથે જોડે છે. અમે તમારી નોકરી શોધમાં મદદ કરવા માટે વિવિધ સાધનો પ્રદાન કરીએ છીએ.";

            if (msg.IndexOf("Careerlink म्हणजे काय", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink हे नोकरी शोधण्यासाठी एक व्यासपीठ आहे जे नोकरी शोधणाऱ्यांना नियोक्त्यांशी जोडते. आम्ही तुमच्या नोकरी शोधण्यात मदत करण्यासाठी विविध साधने प्रदान करतो.";

            if (msg.IndexOf("how to apply for a job", StringComparison.OrdinalIgnoreCase) >= 0)
                return "To apply for a job, click on the job listing and follow the instructions provided. You may need to upload your resume and fill out an application form.";

            if (msg.IndexOf("कैसे आवेदन करें", StringComparison.OrdinalIgnoreCase) >= 0)
                return "किसी नौकरी के लिए आवेदन करने के लिए, नौकरी सूची पर क्लिक करें और दिए गए निर्देशों का पालन करें। आपको अपना रिज्यूमे अपलोड करना और आवेदन पत्र भरना पड़ सकता है।";

            if (msg.IndexOf("કેવી રીતે અરજી કરવી", StringComparison.OrdinalIgnoreCase) >= 0)
                return "નોકરી માટે અરજી કરવા માટે, નોકરીની યાદી પર ક્લિક કરો અને આપેલા સૂચનોનું પાલન કરો. તમારે તમારું રિઝ્યુમ અપલોડ કરવું અને અરજી ફોર્મ ભરવું પડી શકે છે.";

            if (msg.IndexOf("अर्ज कसा करायचा", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नोकरीसाठी अर्ज करण्यासाठी, नोकरी सूचीवर क्लिक करा आणि दिलेल्या सूचनांचे पालन करा. तुम्हाला तुमचं रिझ्युमे अपलोड करावं लागेल आणि अर्ज फॉर्म भरावा लागू शकतो.";

            // Ending
            if (msg.IndexOf("thank you", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("thanks", StringComparison.OrdinalIgnoreCase) >= 0)
                return "You're welcome! Best of luck with your job hunt.";

            if (msg.IndexOf("धन्यवाद", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("शुक्रिया", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आपका स्वागत है! आपकी नौकरी खोज के लिए शुभकामनाएं।";

            if (msg.IndexOf("આભાર", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("ધન્યવાદ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "તમારું સ્વાગત છે! તમારી નોકરી શોધ માટે શુભેચ્છાઓ.";

            if (msg.IndexOf("धन्यवाद", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("थँक्स", StringComparison.OrdinalIgnoreCase) >= 0)
                return "तुमचं स्वागत आहे! तुमच्या नोकरी शोधासाठी शुभेच्छा.";

            if (msg.IndexOf("bye", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("goodbye", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Goodbye! Come back anytime you need career help.";

            if (msg.IndexOf("अलविदा", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("विदा", StringComparison.OrdinalIgnoreCase) >= 0)
                return "अलविदा! जब भी आपको करियर सहायता की आवश्यकता हो, वापस आएं।";

            if (msg.IndexOf("બાય", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("ગુડબાય", StringComparison.OrdinalIgnoreCase) >= 0)
                return "બાય! જ્યારે પણ તમને કારકિર્દી સહાયની જરૂર હોય ત્યારે પાછા આવો.";

            if (msg.IndexOf("बाय", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("गुडबाय", StringComparison.OrdinalIgnoreCase) >= 0)
                return "बाय! जेव्हा तुम्हाला करिअर सहाय्याची गरज असेल तेव्हा परत या.";

            if (msg.IndexOf("see you later", StringComparison.OrdinalIgnoreCase) >= 0)
                return "See you later! Wishing you all the best in your job search.";

            if (msg.IndexOf("फिर मिलेंगे", StringComparison.OrdinalIgnoreCase) >= 0)
                return "फिर मिलेंगे! आपकी नौकरी खोज में आपको शुभकामनाएं।";

            if (msg.IndexOf("પછી મળીએ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "પછી મળીએ! તમારી નોકરી શોધમાં તમને શુભેચ્છાઓ.";

            if (msg.IndexOf("नंतर भेटू", StringComparison.OrdinalIgnoreCase) >= 0)
                return "नंतर भेटू! तुमच्या नोकरी शोधासाठी शुभेच्छा.";

            if (msg.IndexOf("help", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("assist", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I can help you with job searches, resume building, and interview tips. What do you need assistance with?";

            if (msg.IndexOf("मदद", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("सहायता", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मैं आपको नौकरी खोज, रिज्यूमे बनाने और इंटरव्यू टिप्स में मदद कर सकता हूँ। आपको किस चीज़ में सहायता चाहिए?";

            if (msg.IndexOf("મદદ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("સહાય", StringComparison.OrdinalIgnoreCase) >= 0)
                return "હું તમને નોકરી શોધ, રિઝ્યુમ બનાવવામાં અને ઇન્ટરવ્યુ ટીપ્સમાં મદદ કરી શકું છું. તમને કઈ મદદ જોઈએ છે?";

            if (msg.IndexOf("मदत", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("सहाय्य", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मी तुम्हाला नोकरी शोध, रिझ्युमे तयार करणे आणि मुलाखत टिप्समध्ये मदत करू शकतो. तुम्हाला कशाची मदत हवी आहे?";

            if (msg.IndexOf("best ats rating resume template", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Check out our premium templates designed for ATS compatibility. They are available in the Resume Builder section.";

            if (msg.IndexOf("सबसे अच्छा एटीएस रेटिंग रिज्यूमे टेम्पलेट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "हमारे प्रीमियम टेम्पलेट्स देखें जो एटीएस संगतता के लिए डिज़ाइन किए गए हैं। वे रिज्यूमे बिल्डर सेक्शन में उपलब्ध हैं।";

            if (msg.IndexOf("શ્રેષ્ઠ એટીએસ રેટિંગ રિઝ્યુમ ટેમ્પલેટ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "અમારા પ્રીમિયમ ટેમ્પલેટ્સ જુઓ જે એટીએસ સુસંગતતા માટે ડિઝાઇન કરવામાં આવ્યા છે. તે રિઝ્યુમ બિલ્ડર વિભાગમાં ઉપલબ્ધ છે.";

            if (msg.IndexOf("सर्वोत्तम एटीएस रेटिंग रिझ्युमे टेम्पलेट", StringComparison.OrdinalIgnoreCase) >= 0)
                return "आमचे प्रीमियम टेम्पलेट्स पहा जे एटीएस सुसंगततेसाठी डिझाइन केलेले आहेत. ते रिझ्युमे बिल्डर विभागात उपलब्ध आहेत.";

            if (msg.IndexOf("who developed you", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I was developed by the Careerlink team to assist you in your job search.";

            if (msg.IndexOf("आपको किसने विकसित किया", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मुझे Careerlink टीम द्वारा आपकी नौकरी खोज में सहायता करने के लिए विकसित किया गया था।";

            if (msg.IndexOf("તમને કોણે વિકસાવ્યા", StringComparison.OrdinalIgnoreCase) >= 0)
                return "મને Careerlink ટીમ દ્વારા તમારી નોકરી શોધવામાં મદદ કરવા માટે વિકસાવવામાં આવ્યા હતા.";

            if (msg.IndexOf("तुला कोणी विकसित केलं", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मला Careerlink टीमने तुमच्या नोकरी शोधण्यात मदत करण्यासाठी विकसित केलं.";

            if (msg.IndexOf("how are you", StringComparison.OrdinalIgnoreCase) >= 0)
                return "I'm just a bot, but I'm here to help you with your job search!";

            if (msg.IndexOf("आप कैसे हैं", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मैं सिर्फ एक बॉट हूँ, लेकिन मैं आपकी नौकरी खोज में आपकी मदद करने के लिए यहाँ हूँ!";

            if (msg.IndexOf("તમે કેમ છો", StringComparison.OrdinalIgnoreCase) >= 0)
                return "હું ફક્ત એક બોટ છું, પરંતુ હું તમારી નોકરી શોધવામાં મદદ કરવા માટે અહીં છું!";

            if (msg.IndexOf("तू कसा आहेस", StringComparison.OrdinalIgnoreCase) >= 0 ||
                msg.IndexOf("तू कशी आहेस", StringComparison.OrdinalIgnoreCase) >= 0)
                return "मी फक्त एक बॉट आहे, पण मी तुमच्या नोकरी शोधण्यात मदत करण्यासाठी येथे आहे!";

            if (msg.IndexOf("which platform is best for job applying", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink is your best platform for job applications! With ATS-optimized resume tools, real-time job suggestions, and a smart chatbot assistant like me — we make your job search easier and more effective!";

            if (msg.IndexOf("नौकरी आवेदन के लिए सबसे अच्छा प्लेटफॉर्म कौन सा है", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink नौकरी आवेदन के लिए आपका सबसे अच्छा प्लेटफॉर्म है! ATS-अनुकूलित रिज्यूमे टूल्स, रियल-टाइम जॉब सुझाव, और मेरे जैसे स्मार्ट चैटबॉट सहायक के साथ — हम आपकी नौकरी खोज को आसान और अधिक प्रभावी बनाते हैं!";

            if (msg.IndexOf("નોકરી માટે અરજી કરવા માટે શ્રેષ્ઠ પ્લેટફોર્મ કયું છે", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink નોકરી માટે અરજી કરવા માટે તમારું શ્રેષ્ઠ પ્લેટફોર્મ છે! ATS-ઓપ્ટિમાઇઝ્ડ રિઝ્યુમ ટૂલ્સ, રિયલ-ટાઇમ નોકરી સૂચનો, અને મારા જેવા સ્માર્ટ ચેટબોટ સહાયક સાથે — અમે તમારી નોકરી શોધને સરળ અને વધુ અસરકારક બનાવીએ છીએ!";

            if (msg.IndexOf("नोकरीसाठी अर्ज करण्यासाठी सर्वोत्तम प्लॅटफॉर्म कोणता आहे", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Careerlink नोकरीसाठी अर्ज करण्यासाठी तुमचं सर्वोत्तम प्लॅटफॉर्म आहे! ATS-अनुकूलित रिझ्युमे टूल्स, रिअल-टाइम जॉब सूचना, आणि माझ्यासारख्या स्मार्ट चॅटबॉट सहायकाशी — आम्ही तुमची नोकरी शोध अधिक सोपी आणि प्रभावी बनवतो!";

            // if (msg.IndexOf("which platform is best for job applying", StringComparison.OrdinalIgnoreCase) >= 0)
            //     return "Careerlink is your best platform for job applications! With ATS-optimized resume tools, real-time job suggestions, and a smart chatbot assistant like me — we make your job search easier and more effective!";

            // Fallback
            return "I'm here to help with anything related to your job search. Can you rephrase that or ask something else?";
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}