using LaserControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2
{
    public class FunctionLib
    {
        static FunctionLib()
        {
            FunctionLib.Load();
        }


        protected static List<FunctionCategory> AllCategories = new List<FunctionCategory>();
        protected static List<Function> AllFunctions = new List<Function>();

        public static bool Loaded
        {
            get;
            protected set;
        }

        public static void Load()
        {
            if (!Loaded)
            {
                AllCategories.Clear();
                AllFunctions.Clear();

                FunctionCategory AllCat = new FunctionCategory("All Functions");
                //AllCategories.Add(AllCat);

                //1) Lade alle Kategorien!
                DataSafe ds = new Data.DataSafe(Data.Paths.SettingsPath, "Functions");
                int catCount = ds.Ints["Categories", 0];
                for (int i = 0; i < catCount; ++i)
                {
                    string name = ds.Strings["Category-" + i, ""];
                    /*if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception("Error in Function Categories -> Functions.xml");
                    }*/

                    FunctionCategory category = new FunctionCategory(name );


                    DataSafe catSafe = new DataSafe(Data.Paths.SettingsScriptFunctionsPath, "Category-" + name);
                    int funCount = catSafe.Ints["Functions", 0];
                    for (int f = 0; f < funCount; ++f)
                    {
                        string funName = catSafe.Strings["Function-" + f, ""];
                        AllCat.Functions.Add(funName);
                        category.Functions.Add(funName);

                        //Lade eine Funktion
                        AllFunctions.Add( Function.Load(funName) );

                    }

                    category.Sort();
                    AllCategories.Add(category);
                }

                //AllFunctions.Sort();
                AllCat.Sort();

                AllCategories.Sort();
                AllCategories.Insert(0,AllCat);

                

                /*
                string code = "$RETURN = 1;if( $i > 1 ){ $RETURN = FAK($i - 1) * $i; }";
                Function fak = new Function("FAK", code, typeof(int));
                fak.AddParameterInt("I");
                AllFunctions.Add(fak);
                */
                Loaded = true;
            }
            else
            {
                Console.WriteLine("All Functions have been loaded before!");
            }
        }

        public static void Save()
        {
            DataSafe ds = new Data.DataSafe(Data.Paths.SettingsPath, "Functions");
            ds.Ints["Categories"] = AllCategories.Count-1;
            for (int i = 1; i < AllCategories.Count; ++i)
            {
                FunctionCategory category = AllCategories[i];
                ds.Strings["Category-" + (i-1)] = category.Title.ToUpper();
                DataSafe catSafe = new DataSafe(Data.Paths.SettingsScriptFunctionsPath, "Category-" + category.Title);
                catSafe.Ints["Functions"] = category.Functions.Count;
                for (int f = 0; f < category.Functions.Count; ++f)
                {
                    catSafe.Strings["Function-" + f] = category.Functions[f].ToUpper();
                    GetFunction(category.Functions[f]).Save();
                }
            }
        }

        public static FunctionCategory GetCategory(string name)
        {
            name = name.ToUpper();
            if (AllCategories.Count(x => x.Title == name) > 0)
            {
                return AllCategories.Find(x => x.Title == name);
            }
            return null;
        }


        public static Function GetFunction(string name)
        {
            name = name.ToUpper();
            if (AllFunctions.Count(x => x.FunctionName == name) > 0)
            {
                return AllFunctions.Find(x=>x.FunctionName == name);
            }
            return null;
        }

        public static TVStringItem[] GetTreeViewNodes()
        {
            List<TVStringItem> res = new List<TVStringItem>();

            foreach (var c in AllCategories)
            {
                TVStringItem i = new TVStringItem() { Title = c.Title };
                foreach (var f in c.Functions)
                {
                    i.Items.Add(new TVStringItem() { Title = f, IsFunction = true, FunctionName = f });
                }
                res.Add(i);
            }

            return res.ToArray();
        }


        public static void AddCategory(FunctionCategory fc)
        {
            AllCategories.Add(fc);
        }

        public static void AddFunction(string category, Function function)
        {
            FunctionCategory cat = GetCategory(category);
            if(cat != null)
            {
                cat.Functions.Add(function.FunctionName);
                AllCategories[0].Functions.Add(function.FunctionName);
                AllFunctions.Add(function);
            }
        }
    }
}
