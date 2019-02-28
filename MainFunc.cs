using HelloCSharp.MySQL;
using HelloCSharp.MySQL.MySQLPool;
using HelloCSharp.UI;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace HelloCSharp
{
    class MainFunc
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Temp tempClass = JsonConvert.DeserializeObject<Temp>("{" +
            "\"result_code\": \"0\"," +
            "\"message\": \"成功\"," +
            "\"openid\": \"OPENID\"," +
            "\"nick_name\": \"NICKNAME\"," +
            "\"avatar_url\": \"image.com/xxxx.png\"," +
            "\"gender\": \"1\"" +
            "}");
            foreach (System.Reflection.PropertyInfo p in tempClass.GetType().GetProperties())
            {
                Console.WriteLine("Name:{0}\tValue:{1}", p.Name, p.GetValue(tempClass));
            }
            Console.ReadKey();



            //A a = new A
            //{
            //    Name = "Li",
            //    Address = "ShenZheng",
            //    Age = 22,
            //    Flag = true
            //};
            //B b = new B();
            //b.InjectFrom(a);
            //foreach (PropertyDescriptor tempDescriptor in TypeDescriptor.GetProperties(b))
            //{
            //    Console.WriteLine("Name:{0}Value:{1}", tempDescriptor.Name, tempDescriptor.GetValue(b));
            //}
            //byte charA = (byte)'a';
            //Console.WriteLine(charA);
            //Console.WriteLine(charA.ToString("X2"));
            //Console.ReadKey();



            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //MySqlUtil mySqlUtil = new MySqlUtil();
            //Application.Run(new LoginAndRegisterWindow());
            //Application.Run(new MainForm());
            //Application.Run(new FileB());



            //TestConnectionPool t = new TestConnectionPool();
            //t.Test();
        }

    }

    class Temp
    {
        public string Result_code { get; set; }
        public string Message { get; set; }
        public string Openid { get; set; }
        public string Nick_name { get; set; }
        public string Avatar_url { get; set; }
        public string Gender { get; set; }
    }

    class A
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public bool Flag { get; set; }
    }

    class B
    {
        public string OldName { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public bool Flag { get; set; }
    }
}
