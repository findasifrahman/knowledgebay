using KioskNavy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Globalization;
using NAudio.Lame;
using System.Threading;
using NAudio.Wave;

namespace KioskNavy.Controllers
{

    public class UserController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        private UserDBContext userdb = new UserDBContext();
        // GET: User
        public ActionResult choice(string Name, string code, string rank)
        {
            ViewBag.User = Name;
            ViewBag.Code = code;
            ViewBag.Rank = rank;
            return View();
        }
        public ActionResult commonPage()
        {
            return View();
        }
        public ActionResult loginsuccess(string name, string rank)
        {
            ViewBag.User = name;
            ViewBag.Rank = rank;
            return View();
        }
        public ActionResult subjectpopup(string name, string rank)
        {
            ViewBag.User = name;
            ViewBag.Rank = rank;
            return View();
        }
        public ActionResult subsubjectpopup(string name, string rank,string subject)
        {
            ViewBag.User = name;
            ViewBag.Rank = rank;
            ViewBag.subject = subject;
            return View();
        }
        public ActionResult levelpopup(string name, string rank, string subject,string subsubject)
        {
            ViewBag.User = name;
            ViewBag.Rank = rank;
            ViewBag.subject = subject;
            ViewBag.subsubject = subsubject;
            return View();
        }
        public ActionResult Index(string Name,string code,string rank)
        {
            Session["currentUser"] = Name;
            Session["currentCode"] = code;
            Session["currentRank"] = rank;
            ViewBag.User = Name;
            ViewBag.Code = code;
            ViewBag.Rank = rank;
            return View(db.QuizNameModels.ToList());
        }
        public ActionResult test()
        {
            return View();
        }
        public ActionResult SubSubjectIndex(string QuizName, string Name, string code, string rank)
        {
            System.Diagnostics.Debug.WriteLine("------------------------- ------------------ -----" + QuizName);
            int toy = 0;
            ViewBag.User = Name;
            ViewBag.Code = code;
            ViewBag.Rank = rank;
            ViewBag.numoflevel = toy;
            ViewBag.subject = QuizName;
            ViewBag.Quizname = QuizName;
            Session["currentUser"] = Name;
            Session["currentCode"] = code;
            Session["currentRank"] = rank;
            ViewBag.User = Session["currentUser"];
            return View(db.subSubjectmdels.Where(x => x.SubjectName == QuizName).ToList());
        }
        public ActionResult LevelIndex(string QuizName,string subsubject, string Name,string code, string rank)
        {
            int toy = 0;
            ViewBag.User = Name;
            ViewBag.Code = code;
            ViewBag.Rank = rank;
            ViewBag.numoflevel = toy;
            ViewBag.SubSubject = subsubject;
            ViewBag.Quizname = QuizName;
            Session["currentUser"] = Name;
            Session["currentCode"] = code;
            Session["currentRank"] = rank;
            ViewBag.User = Session["currentUser"];
            return View(db.LevelNameModels.Where(x=> x.SubjectName == QuizName).ToList());
        }
        [HttpPost]
        public ActionResult subexist(string subject)
        {
            var subexist =  db.subSubjectmdels.Where(x => x.SubjectName == subject).FirstOrDefault();
            var levelexist = db.LevelNameModels.Where(x => x.SubjectName == subject).FirstOrDefault();
            string se = "true";
            string le = "true";

            if(subexist == null)
            {
                se = "false";
            }
            if(levelexist == null)
            {
                le = "false";
            }
            return Json(new { Success = "true", Data = new { SubExist = se, LevExist = le} });
        }
        [HttpPost]
        public ActionResult gettotalq(string id, string code, string level,string subsubject)
        {
            int thres = db.LevelNameModels.Where(x => x.Level == level && x.SubjectName == id ).Select(x => x.certificateTreshhold).FirstOrDefault();
            string s = userdb.UserConditionModels.Where(x => x.quizName == id && x.mCode == code && x.subjectLevel == level && x.SubSubject == subsubject).Select(x => x.allAnswers).FirstOrDefault();
            var final = 0;
            if (s == null)
            {
                final = 0;
            }
            else if (s == "0")
            {
                final = 0;
            }
            else
            {
                string[] values = s.Split(',');
                List<int> a = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    a.Add(Convert.ToInt32(values[i]));
                }

                ////////////////////////
                final = a.Count - 1;
            }
            if (final > thres)
            {
                return Json(new { Success = "true", Data = new { rand = final.ToString(), equals = "false", level = "true" } });
            }
            else if (final == thres)
            {
                return Json(new { Success = "true", Data = new { rand = final.ToString(), equals = "true", level = "true" } });
            }
            return Json(new { Success = "false", Data = new { rand = final.ToString(), equals = "false", level = "false" } });
        }
        [HttpPost]
        public ActionResult getrandom(string id, string level, string code, string curqn, string ans,string subsubject)
        {
            //username = Session["currentUser"].ToString();
            if (code == null)
            {
                code = Session["currentCode"].ToString();
            }
            int final = 1;
            string s = userdb.UserConditionModels.Where(x => x.quizName == id && x.mCode == code && x.subjectLevel == level && x.SubSubject == subsubject).Select(x => x.allAnswers).FirstOrDefault(); //"1,2,4,6,8,9,10,11,18";
            if (s != null)
            {
                final = GiveMeANumber(s, id, level, curqn, ans, subsubject);

                ///////////////////
            }
            else
            {
                int maxNumber = db.QuizQuestionModels.Where(x => x.QuizName == id && x.Level == level && x.SubSubject == subsubject).Max(p => (int?)p.QuestionNumber) ?? 1;
                if (ans == "0")
                {

                    // var range = Enumerable.Range(1, maxNumber);
                    var rand = new System.Random();
                    int index = rand.Next(1, maxNumber);
                    final = index;//range.ElementAt(index);
                }
                ///////////////////////////////////////
                if (ans == "1")
                {
                    //string[] values = s.Split(',');
                    List<int> a = new List<int>();

                    a.Add(Convert.ToInt32(curqn));

                    ////////////////////////
                    var range = Enumerable.Range(1, maxNumber).Where(i => !a.Contains(i));
                    var rand = new System.Random();
                    int index = rand.Next(0, maxNumber - (a.Count ));
                    final = range.ElementAt(index);

                    System.Diagnostics.Debug.WriteLine("mwe are inside very firtd ------------------ -----" + maxNumber.ToString());
                    System.Diagnostics.Debug.WriteLine("mwe are inside very firtd ------------------ -----" + final.ToString());

                }
                ///////////////////////////////////////
            }
            ////////////////////////
            bool finalexist = true;
            while (finalexist && final != -1)
            {
                var yy = db.QuizQuestionModels.Where(x => x.QuestionNumber == final && x.QuizName == id && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();
                if (yy == null)
                {
                    final = GiveMeANumber(s, id, level, curqn, ans,subsubject);
                    finalexist = true;
                }
                else
                {
                    finalexist = false;
                }
            }
            if (final == -1)
            {
                return Json(new { Success = "true", Data = new { rand = "comp", level = code } });
            }
            ////////////////////////
            return Json(new { Success = "true", Data = new { rand = final.ToString(), level = code } });
        }
        private int GiveMeANumber(string s, string id, string level, string curqn, string ans,string subsubject)
        {
            int final = 1;
            int maxNumber = db.QuizQuestionModels.Where(x => x.QuizName == id && x.Level == level && x.SubSubject == subsubject).Max(p => (int?)p.QuestionNumber) ?? 1;
            if(s == null)
            {
                var rand = new System.Random();
                int index = rand.Next(1, maxNumber);
                final = index;
                return final;
            }
            else if (s.Equals("0"))
            {
                var rand = new System.Random();
                int index = rand.Next(1, maxNumber);
                final = index;
                return final;
            }
            if (s != null)
            {
                string[] values = s.Split(',');
                List<int> a = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    a.Add(Convert.ToInt32(values[i]));


                    System.Diagnostics.Debug.WriteLine("a elements");
                    System.Diagnostics.Debug.WriteLine(a[i]);
                }
                if (Convert.ToInt32(ans) == 1)// correct answer
                {
                    a.Add(Convert.ToInt32(curqn));

                    int totalQues = db.QuizQuestionModels.Where(x => x.QuizName == id && x.Level == level && x.SubSubject == subsubject).ToList().Count();
                    System.Diagnostics.Debug.WriteLine("------------------ Total question inside give me Fina--" + totalQues);
                    if ((a.Count - 1) >= totalQues)
                    {
                        return -1;
                    }
                    var range = Enumerable.Range(0, maxNumber + 1).Where(i => !a.Contains(i));
                    /////////////////////////////////////////
                    int rangelen = 0;
                    foreach (var i in range)
                    {
                        System.Diagnostics.Debug.WriteLine("range elements");
                        System.Diagnostics.Debug.WriteLine(i);
                        rangelen++;
                    }
                    //System.Diagnostics.Debug.WriteLine(range)
                    ///////////////////////////////////////
                    var rand = new System.Random();
                    int index = rand.Next(0, maxNumber - (a.Count));
                    if (rangelen == 1)
                    {
                        final = range.ElementAt(0);
                    }
                    else
                    {
                        final = range.ElementAt(index);
                    }
                }
                ////////////////////////
                else
                {
                    var range = Enumerable.Range(0, maxNumber + 1).Where(i => !a.Contains(i));
                    int rangelen = 0;
                    foreach (object ss in range)
                    {
                        System.Diagnostics.Debug.WriteLine("range elements");
                        System.Diagnostics.Debug.WriteLine(ss);
                        rangelen++;
                    }
                    var rand = new System.Random();
                    int index = rand.Next(0, maxNumber - (a.Count - 1));
                    if (rangelen == 1)
                    {
                        final = range.ElementAt(0);
                    }
                    else
                    {
                        final = range.ElementAt(index);
                    }
                }
            }
            return final;
        }
        public int numofAll(string id, string level, string code,string subsubject)
        {
            AdminDBContext db1 = new AdminDBContext();
            UserDBContext userdb1 = new UserDBContext();
            string s = userdb1.UserConditionModels.Where(x => x.quizName == id && x.mCode == code && x.subjectLevel == level && x.SubSubject == subsubject).Select(x => x.allAnswers).FirstOrDefault();
            int index = 0;

            int maxNumber = db1.QuizQuestionModels.Where(x => x.QuizName.Equals(id) && x.Level == level && x.SubSubject == subsubject).Max(p => (int?)p.QuestionNumber) ?? 1;
            System.Diagnostics.Debug.WriteLine("max number is -----" + maxNumber.ToString());
            if (s == null)
            {

                System.Diagnostics.Debug.WriteLine("quiz name -----" + id);
                // var range = Enumerable.Range(1, maxNumber);
                var rand = new System.Random();
                index = rand.Next(1, maxNumber);
            }
            else if (s.Equals('0'))
            {
                System.Diagnostics.Debug.WriteLine("quiz name -----" + id);
                // var range = Enumerable.Range(1, maxNumber);
                var rand = new System.Random();
                index = rand.Next(1, maxNumber);
            }

            /////////////////////////////
            else if (s != null)
            {
                string[] values = s.Split(',');
                List<int> a = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    a.Add(Convert.ToInt32(values[i]));
                    System.Diagnostics.Debug.WriteLine(a[i]);
                }
                ////////////////////////
                var range = Enumerable.Range(1, maxNumber).Where(i => !a.Contains(i));
                var rand = new System.Random();
                index = rand.Next(1, maxNumber - (a.Count - 1));
                //////////////
                int rangelen = 0;
                foreach (var i in range)
                {
                    System.Diagnostics.Debug.WriteLine("range elements");
                    System.Diagnostics.Debug.WriteLine(i);
                    rangelen++;
                }

                if (rangelen == 1)
                {
                    index = range.ElementAt(0);
                }
                else
                {
                    index = range.ElementAt(index);
                }
                //////////////
                //index = range.ElementAt(index);
            }
            /////////////////////////////
            bool finalexist = true;
            while (finalexist)
            {
                var yy = db.QuizQuestionModels.Where(x => x.QuestionNumber == index && x.Level == level && x.QuizName == id && x.SubSubject == subsubject).FirstOrDefault();
                if (yy == null)
                {
                    var rand = new System.Random();
                    index = rand.Next(1, maxNumber);
                    finalexist = true;
                    ///////////////////////
                    if (s == null)
                    {

                        System.Diagnostics.Debug.WriteLine("quiz name -----" + id);
                        // var range = Enumerable.Range(1, maxNumber);
                        //var rand = new System.Random();
                        index = rand.Next(1, maxNumber);
                    }
                    else if (s.Equals('0'))
                    {
                        System.Diagnostics.Debug.WriteLine("quiz name -----" + id);
                        // var range = Enumerable.Range(1, maxNumber);
                        //var rand = new System.Random();
                        index = rand.Next(1, maxNumber);
                    }

                    /////////////////////////////
                    else if (s != null)
                    {
                        string[] values = s.Split(',');
                        List<int> a = new List<int>();
                        for (int i = 0; i < values.Length; i++)
                        {
                            a.Add(Convert.ToInt32(values[i]));
                            System.Diagnostics.Debug.WriteLine(a[i]);
                        }
                        ////////////////////////
                        var range = Enumerable.Range(1, maxNumber).Where(i => !a.Contains(i));
                        //var rand = new System.Random();
                        index = rand.Next(1, maxNumber - (a.Count - 1));
                        index = range.ElementAt(index);
                    }
                    ////////////////////////
                }
                else
                {
                    finalexist = false;
                    break;
                }
            }
            return index;

        }
        public int numofcount(string id, string code, string level,string subsubject)
        {
            string s = userdb.UserConditionModels.Where(x => x.quizName == id && x.mCode == code && x.subjectLevel == level && x.SubSubject == subsubject).Select(x => x.allAnswers).FirstOrDefault();
            if (s == null)
            {
                return 0;
            }
            if (s == "0")
            {
                return 0;
            }
            else
            {
                string[] values = s.Split(',');
                List<int> a = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    a.Add(Convert.ToInt32(values[i]));
                }

                ////////////////////////
                return a.Count - 1;
            }
        }
        public int numofSecondAttemp(string id, string code, string level,string subsubject)
        {
            string s = userdb.UserConditionModels.Where(x => x.quizName == id && x.mCode == code && x.subjectLevel == level && x.SubSubject ==subsubject).Select(x => x.userXP).FirstOrDefault();
            if (s == null)
            {
                return 0;
            }
            if (s == "0")
            {
                return 0;
            }
            else
            {
                string[] values = s.Split(',');
                List<int> a = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    a.Add(Convert.ToInt32(values[i]));
                }

                ////////////////////////
                return a.Count - 1;
            }
        }
        [HttpGet]
        public ActionResult QuestionDetail(string choice,string level,string subsubject, string id3,string id1,string prev,string secondAttempt,string mname,string mcode,string mrank)
        {
            //id3=quiz name
            //id1 == QuestionDetail number
            if (mcode == null)//Session["currentUser"] == null)
            {
                return RedirectToAction("../home");
            }

            string usercode =   mcode;//Session["currentCode"];
            string username = mname;//Session["currentUser"].ToString();
            string userrank = mrank;//Session["currentRank"].ToString();
            ViewBag.User = userrank + " " + username;
            ViewBag.code = usercode;
            ViewBag.names = username;
            ViewBag.rank = userrank;
            ViewBag.SubSubject = subsubject;

            if (usercode == null)
            {
                return RedirectToAction("../home");
            }

            int yu = numofcount(id3, usercode, level,subsubject); // how many question participated
            ViewBag.quescompleted = yu.ToString();
            if(secondAttempt == null)
            {
                ViewBag.secondAttemp = numofSecondAttemp(id3, usercode, level,subsubject).ToString();
            }
            if ( secondAttempt == "0")
            {
                ViewBag.secondAttemp = numofSecondAttemp(id3, usercode, level,subsubject).ToString();
            }
            else if(secondAttempt == "1")
            {
                ViewBag.secondAttemp = (numofSecondAttemp(id3, usercode, level,subsubject) + 1).ToString();
            }
            //id3=quiz name
            int how_many_question = numofcount(id3, usercode, level,subsubject);
            var certhreshhold = db.LevelNameModels.Where(x => x.Level == level && x.SubjectName == id3).Select(x => x.certificateTreshhold).FirstOrDefault();
            if(certhreshhold != 0)
            {

            }

            var userdbins = userdb.UserConditionModels.Where(x => x.quizName == id3 && x.mCode == usercode && x.subjectLevel == level && x.SubSubject == subsubject).FirstOrDefault();
            int totalQues = db.QuizQuestionModels.Where(x => x.QuizName == id3 && x.Level == level && x.SubSubject == subsubject).ToList().Count();
            if (numofcount(id3, usercode, level,subsubject) >= totalQues)
            {
                if (id3 == null)
                {
                    return RedirectToAction("../home");
                }
                System.Diagnostics.Debug.WriteLine("------------------Subsubject--" + subsubject);// sub subject
                System.Diagnostics.Debug.WriteLine("------------------Subsubject--" + subsubject);// sub subject

                System.Diagnostics.Debug.WriteLine("dhoni performance poor techniquessfjdjf dfdsfdsfdsfsdf--------" + id3 + "--" + usercode+ "--" + level + "--toq" + totalQues);
                return Redirect("complete?" + "quiz=" + id3 + "&level=" + level + "&subsubject=" + subsubject + "&user=" + mname +
                    "&code=" + mcode + "&rank=" + mrank);
            }

            if (id1.Equals("default"))
            {
                id1 = numofAll(id3, level, usercode,subsubject).ToString();
                int qn = Convert.ToInt32(id1);
                var nn1 = db.QuizQuestionModels.Where(x => x.QuizName.Equals(id3) && x.QuestionNumber == qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();
                return View("QuestionDetail",nn1);//"QuestionDetail?&choice=all&level=" +level + "&id3=" + id3 + "&id1=" + (id1).ToString());
            }

            ViewBag.code = usercode;
            ViewBag.names = username;
            System.Diagnostics.Debug.WriteLine("subjectr--" + id3);
            System.Diagnostics.Debug.WriteLine("user--" + usercode);// quiz name
            System.Diagnostics.Debug.WriteLine("subjectLevel--" + level);// questin number
          
            var final_qn = "0";
            if(userdbins == null)
            {
                final_qn = numofAll(id3, level, usercode,subsubject).ToString();
                UserDBContext userdb1 = new UserDBContext();
                var model = new UserConditionModels();
                model.quizName = id3;
                model.subjectLevel = level;
                model.mCode = usercode;
                model.mName = username;
                model.mRank = userrank;


                model.CurrentDate = DateTime.Now;
                model.SubSubject = subsubject;
                model.wrongAnswers = "";
                if (Convert.ToInt32(choice) == 1)
                {
                    model.allAnswers = "0" + "," + prev; // quiestion number
                    model.currenLevel = "1";//first attempt answer
                    model.prevlevel = "0";//second attempt answer
                }
                else
                {
                    model.wrongAnswers = prev;
                    model.currenLevel = "0";//first attempt answer
                    model.prevlevel = "1";//second attempt answer

                }
                //////////////////////////////////////
                if (Convert.ToInt32(secondAttempt) == 1)
                {
                    model.userXP = "0" + "," + prev;
                }

                //////////////////////////////////////
                    userdb1.UserConditionModels.Add(model);
                userdb1.SaveChanges();
                   
            }
            else if (!choice.Equals("all"))
            {
                UserDBContext userdb1 = new UserDBContext();
                var model = userdb1.UserConditionModels.Where(x => x.mCode == usercode && x.quizName == id3 && x.subjectLevel == level && x.SubSubject == subsubject).FirstOrDefault();
                var prevwronganswer = model.wrongAnswers;
                var prevallanswer = model.allAnswers;
                var prevsecondattemp = model.userXP;
                var prevxp = model.userXP;
                model.quizName = id3;
                model.mName = username;
                model.mCode = usercode;
                model.mRank = userrank;
                int totalallans = Convert.ToInt32(model.currenLevel);
                int totalwrongans = Convert.ToInt32(model.prevlevel);
                if (Convert.ToInt32(choice) == 1)
                {
                    ////////////////////
                    int allready_exist = 0;
                    if (prevallanswer != null)
                    {
                        string[] values = prevallanswer.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            System.Diagnostics.Debug.WriteLine("prevallanswer..............--" + values[i]);
                            if (values[i] == prev)
                            {
                                allready_exist = 1;
                                break;
                            }
                        }
                    }
                    ////////////////////
                    if (prevallanswer == null)
                    {
                        model.allAnswers = "0" + "," + prev;
                        totalallans = totalallans + 1;
                        model.currenLevel = totalallans.ToString();
                    }
                    else if (allready_exist == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("val allready exist..............--");
                    }
                    else
                    {
                        model.allAnswers = prevallanswer + "," + prev; // quiestion number
                        totalallans = totalallans + 1;
                        model.currenLevel = totalallans.ToString();
                    }
                }
                else
                {
                    ////////////////////
                    int allready_exist = 0;
                    if (prevwronganswer !=null)
                    {
                        string[] values = prevwronganswer.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            System.Diagnostics.Debug.WriteLine("prevwronganswer..............--" + values[i]);
                            if (values[i] == prev)
                            {
                                allready_exist = 1;
                                break;
                            }
                        }
                    }
                    ////////////////////
                    if (prevwronganswer.Equals(""))
                    {
                        model.wrongAnswers = prev;
                        totalwrongans = totalwrongans + 1;
                        model.prevlevel = totalwrongans.ToString();
                    }
                    else if(allready_exist == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("val allready exist prevwronganswer..............--");
                    }
                    else
                    {
                        model.wrongAnswers = prevwronganswer + "," + prev;
                        totalwrongans = totalwrongans + 1;
                        model.prevlevel = totalwrongans.ToString();
                    }
                }
                ///////////////////////////////////
                if (Convert.ToInt32(secondAttempt) == 1)
                {
                    ////////////////////
                    int allready_exist = 0;
                    if (prevsecondattemp != null)
                    {
                        string[] values = prevsecondattemp.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            System.Diagnostics.Debug.WriteLine("prevsecondattemp..............--" + values[i]);
                            if (values[i] == prev)
                            {
                                allready_exist = 1;
                                break;
                            }
                        }
                    }
                    ////////////////////
                    if (prevsecondattemp == null)
                    {
                        model.userXP = "0" + "," + prev;
                    }
                    else if (allready_exist == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("val allready exist..............--");
                    }
                    else
                    {
                        model.userXP = prevsecondattemp + "," + prev; // quiestion number
                    }
                }

                //////////////////////////////////  
                model.CurrentDate = DateTime.Now;

                userdb1.Entry(model).State = EntityState.Modified;
                userdb1.SaveChanges();
            }
            if (id1.Equals("comp"))
            {
                System.Diagnostics.Debug.WriteLine("id1 equals comp...... insideeeee" );// quiz name
                return Redirect("complete?" + "quiz=" + id3 + "&level=" + level + "&subsubject=" + subsubject + "&user=" + mname + 
                    "&code=" + mcode + "&rank=" + mrank);
            }
            int ff_final_qn = Convert.ToInt32(id1);//numofAll(id3, level, userm).ToString(); // final random question number

             int yu1 = numofcount(id3,usercode,level,subsubject); // how many question participated
            ViewBag.quescompleted = yu1.ToString();
            System.Diagnostics.Debug.WriteLine("num of count -------------------------------" + yu);// quiz name

            var nn = db.QuizQuestionModels.Where(x => x.QuizName.Equals(id3) && x.QuestionNumber == ff_final_qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();
            
            if (nn != null)
            {
                System.Diagnostics.Debug.WriteLine("nn value will be------------------d--" + nn.QuestionNumber.ToString());
                return View("QuestionDetail", nn);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ha ha ha ha------------------d--" + id1);
                return Redirect("Index");
            }                    
        }
        public ActionResult rightanswer()
        {
            return View();
        }
        public ActionResult wronganswer()
        {
            return View();
        }
        public ActionResult QuestionPopup(string qname, string subsubject, string level, string quesno,string user,string code,string rank)
        {
            ViewBag.User = user;
            ViewBag.Rank = rank;
            int qn = Convert.ToInt32(quesno);
            var nn = db.QuizQuestionModels.Where(x => x.QuizName.Equals(qname) && x.QuestionNumber == qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();

            return View(nn);
        }
        public ActionResult QuestionKnowMore(string qname,string subsubject,string level,string quesno)
        {
            int qn = Convert.ToInt32(quesno);
            var nn = db.QuizQuestionModels.Where(x => x.QuizName.Equals(qname) && x.QuestionNumber == qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();

            return View(nn);
        }
        public ActionResult KnowMoreWithoutvid(string qname, string subsubject, string level, string quesno)
        {
            int qn = Convert.ToInt32(quesno);
            var nn = db.QuizQuestionModels.Where(x => x.QuizName.Equals(qname) && x.QuestionNumber == qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();

            return View(nn);
        }
        public ActionResult KnowMoreWitImage(string qname, string subsubject, string level, string quesno)
        {
            int qn = Convert.ToInt32(quesno);
            var nn = db.QuizQuestionModels.Where(x => x.QuizName.Equals(qname) && x.QuestionNumber == qn && x.Level == level && x.SubSubject == subsubject).FirstOrDefault();

            return View(nn);
        }
        public ActionResult PlayTextArea(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                text = "Type something in first";
            }
            return TextToMp3(text);
        }
        public FileResult TextToMp3(string text)
        {
            //Primary memory stream for storing mp3 audio
            var mp3Stream = new MemoryStream();
            //Speech format
            var speechAudioFormatConfig = new SpeechAudioFormatInfo
            (samplesPerSecond: 8000, bitsPerSample: AudioBitsPerSample.Sixteen,
            channel: AudioChannel.Stereo);
            //Naudio's wave format used for mp3 conversion. 
            //Mirror configuration of speech config.
            var waveFormat = new WaveFormat(speechAudioFormatConfig.SamplesPerSecond,
            speechAudioFormatConfig.BitsPerSample, speechAudioFormatConfig.ChannelCount);
            try
            {
                //Build a voice prompt to have the voice talk slower 
                //and with an emphasis on words
                var prompt = new PromptBuilder
                { Culture = CultureInfo.CreateSpecificCulture("en-GB") };


                prompt.StartVoice(prompt.Culture);
                prompt.StartSentence();
                prompt.StartStyle(new PromptStyle()
                { Emphasis = PromptEmphasis.Reduced, Rate = PromptRate.Medium });
                prompt.AppendText(text);
                prompt.EndStyle();
                prompt.EndSentence();
                prompt.EndVoice();

                //Wav stream output of converted text to speech
                using (var synthWavMs = new MemoryStream())
                {
                    //Spin off a new thread that's safe for an ASP.NET application pool.
                    var resetEvent = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(arg =>
                    {
                        try
                        {
                            //initialize a voice with standard settings
                            var siteSpeechSynth = new SpeechSynthesizer();
                            //Set memory stream and audio format to speech synthesizer
                            siteSpeechSynth.SelectVoiceByHints(VoiceGender.Female,VoiceAge.Adult);

                            siteSpeechSynth.SetOutputToAudioStream
                                (synthWavMs, speechAudioFormatConfig);
                            //build a speech prompt
                            siteSpeechSynth.Speak(prompt);
                        }
                        catch (Exception ex)
                        {
                            //This is here to diagnostic any issues with the conversion process. 
                            //It can be removed after testing.
                            Response.AddHeader
                            ("EXCEPTION", ex.GetBaseException().ToString());
                        }
                        finally
                        {
                            resetEvent.Set();//end of thread
                        }
                    });
                    //Wait until thread catches up with us
                    WaitHandle.WaitAll(new WaitHandle[] { resetEvent });
                    //Estimated bitrate
                    var bitRate = (speechAudioFormatConfig.AverageBytesPerSecond * 8);
                    //Set at starting position
                    synthWavMs.Position = 0;
                    //Be sure to have a bin folder with lame dll files in there. 
                    //They also need to be loaded on application start up via Global.asax file
                    using (var mp3FileWriter = new LameMP3FileWriter
                    (outStream: mp3Stream, format: waveFormat, bitRate: bitRate))
                        synthWavMs.CopyTo(mp3FileWriter);
                }
            }
            catch (Exception ex)
            {
                Response.AddHeader("EXCEPTION", ex.GetBaseException().ToString());
            }
            finally
            {
                //Set no cache on this file
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                //required for chrome and safari
                Response.AppendHeader("Accept-Ranges", "bytes");
                //Write the byte length of mp3 to the client
                Response.AddHeader("Content-Length",
                    mp3Stream.Length.ToString(CultureInfo.InvariantCulture));
            }
            //return the converted wav to mp3 stream to a byte array for a file download
            return File(mp3Stream.ToArray(), "audio/mp3");
        }
        public ActionResult complete(string quiz,string level,string subsubject,string user,string code, string rank)
        {
            System.Diagnostics.Debug.WriteLine("inside complete------------------d--" + quiz);
            ViewBag.user = user;//Session["currentUser"];
            ViewBag.code = code;
            ViewBag.rank = rank;
            ViewBag.level = level;
            ViewBag.quiz = quiz;
            ViewBag.subsubject = subsubject;
            return View();
        }
        [Authorize]
        public ActionResult ReportViewIndex()
        {
            ViewBag.users = new SelectList(userdb.UserConditionModels.ToList(), "mCode", "mCode");
            return View();
        }
        [Authorize]
        public ActionResult AllUserReport()
        {
            ViewBag.Layout = "~/Views/Shared/_ReportLayout.cshtml";
            return View(userdb.UserConditionModels.ToList().OrderBy(x=> x.mCode));
        }
        [Authorize]
        public ActionResult UserReport(string mCode)
        {
            ViewBag.Layout = "~/Views/Shared/_ReportLayout.cshtml";
            return View(userdb.UserConditionModels.Where(x => x.mCode == mCode).ToList().OrderBy(x => x.CurrentDate));
        }
        public ActionResult ViewProgress(string name, string code, string rank)
        {
            ViewBag.name = name;
            ViewBag.code = code;
            ViewBag.rank = rank;
            UserDBContext userdb = new UserDBContext();
            return View(userdb.UserConditionModels.Where(m => m.mCode == code).ToList());
        }
        [HttpPost]
        public ActionResult Reset(string quizname,string level ,string subsubject)
        {

            string user = Session["currentUser"].ToString();

            string usercode = ViewBag.User = Session["currentCode"];
            string username = Session["currentUser"].ToString();
            string userrank = Session["currentRank"].ToString();
  
            if (usercode == null)
            {
                return RedirectToAction("../home");
            }

            if (quizname != null)
            {
                UserDBContext userdb1 = new UserDBContext();
                var model = userdb1.UserConditionModels.Where(x => x.mCode == usercode && x.quizName == quizname && x.subjectLevel == level && x.SubSubject == subsubject).FirstOrDefault();
                
                model.prevlevel = "1";
                model.currenLevel = "1";
                model.wrongAnswers = "";
                model.allAnswers = null;
                model.userXP = "0";
                
                
                userdb1.Entry(model).State = EntityState.Modified;
                userdb1.SaveChanges();
                

                return Json(new { Success = "true", Data = new { quizname = quizname } });
            }
            return Json(new { Success = "false" });
        }
        ///////////////////////////////
        [HttpPost]
        public JsonResult AutoComplete(string Prefix)
        {
            var customers = (from customer in db.subSubjectmdels
                             where customer.SubSubject.StartsWith(Prefix)
                             select new
                             {
                                 label = customer.SubSubject,
                                 ssubject = customer.SubjectName
                             }).ToList().Take(5);//db.LearningModels.SelectMany(x => x.TopicName).ToList();
            return Json(customers);
        }
        ////////////////////////////////

    }
}