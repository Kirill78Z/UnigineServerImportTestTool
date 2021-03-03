using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unigine;

namespace UnigineServerImportTestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Unigine.Engine.Init(/*args*/new string[]
            {
                "-data_path", "../", "-video_app", "null", "-extern_plugin", "Importers/FbxImporter,Importers/IfcImporter",
            });

            System.Console.WriteLine();
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("Unigine is initialized!!!");
            System.Console.WriteLine("Start importing files");

            if (System.IO.Directory.Exists("../test_files"))
            {
                var files = System.IO.Directory.GetFiles("../test_files");
                foreach (var file in files)
                {
                    string ext = System.IO.Path.GetExtension(file).ToLowerInvariant();
                    if (!ext.Equals(".ifc") && !ext.Equals(".fbx"))
                        continue;

                    System.Console.WriteLine("Importing " + file);

                    string fn = System.IO.Path.GetFileNameWithoutExtension(file);

                    if (System.IO.Directory.Exists("../data/" + fn))
                        System.IO.Directory.Delete("../data/" + fn, true);
                    System.IO.Directory.CreateDirectory("../data/" + fn);

                    //препроцессоры
                    List<string> addPreProcessors = new List<string>();

                    //параметры int
                    List<string> setParameterInt = new List<string>();
                    List<int> setParameterIntVal = new List<int>();

                    //параметры float
                    List<string> setParameterFloat = new List<string>();
                    List<float> setParameterFloatVal = new List<float>();

                    //параметры double
                    List<string> setParameterDouble = new List<string>();
                    List<double> setParameterDoubleVal = new List<double>();

                    //параметры string
                    List<string> setParameterString = new List<string>();
                    List<string> setParameterStringVal = new List<string>();





                    if (ext.Equals(".fbx"))
                    {
                        addPreProcessors.Add("MergeSimilarMaterials");
                        addPreProcessors.Add("Repivot");

                        StringArrayPrep(addPreProcessors);

                        //параметры int
                        setParameterInt.AddRange(new string[]
                        { "vertex_cache", "need_triangulate", "workflow", "use_instances", "skip_empty_nodes" });
                        StringArrayPrep(setParameterInt);

                        setParameterIntVal.AddRange(new int[] { 1, 1, 1, 1, 1 });
                    }


                    int memoryUsed = 0;
                    long vectorPtrParam = 0;
                    long charArr = 0;

                    int result = ImportCreateFiles(
                        Encoding.Default.GetString(Encoding.UTF8.GetBytes(file)),
                        Encoding.Default.GetString(Encoding.UTF8.GetBytes(fn + "/")),
                        addPreProcessors.ToArray(), addPreProcessors.Count,
                            setParameterInt.ToArray(), setParameterIntVal.ToArray(), setParameterInt.Count,
                            setParameterFloat.ToArray(), setParameterFloatVal.ToArray(), setParameterFloat.Count,
                            setParameterDouble.ToArray(), setParameterDoubleVal.ToArray(), setParameterDouble.Count,
                            setParameterString.ToArray(), setParameterStringVal.ToArray(), setParameterString.Count,

                            ref memoryUsed, ref vectorPtrParam, ref charArr
                        );


                    System.Console.WriteLine("------------------------------------");
                    switch (result)
                    {
                        case 0:
                            System.Console.WriteLine("Import successful");
                            break;
                        default:
                            System.Console.WriteLine("Something went wrong");
                            break;
                    }

                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    System.Console.WriteLine();


                }
            }

            Engine.Shutdown();
        }

        private static void StringArrayPrep(List<string> str_arr)
        {
            for (int i = 0; i < str_arr.Count; i++)
            {
                str_arr[i] = Encoding.Default
                    .GetString(Encoding.UTF8.GetBytes(str_arr[i]));
            }
        }

        [DllImport(@"ImportLauncherLib_2_11_0_2.dll",
            EntryPoint = "ImportCreateFiles", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImportCreateFiles(
            [MarshalAs(UnmanagedType.AnsiBStr)] string import_path,
            [MarshalAs(UnmanagedType.AnsiBStr)] string output_dir,
            string[] addPreProcessors, int addPreProcessors_count,//препроцессоры

            //параметры импорта
            string[] setParameterInt, int[] setParameterIntVal, int setParameterInt_count,
            string[] setParameterFloat, float[] setParameterFloatVal, int setParameterFloat_count,
            string[] setParameterDouble, double[] setParameterDoubleVal, int setParameterDouble_count,
            string[] setParameterString, string[] setParameterStringVal, int setParameterString_count,

            //получение атрибутивных данных (только для IFC)
            ref int memoryUsed,
            ref long vectorPtrParam,
            ref long charArr
            );



        [DllImport(@"ImportLauncherLib_2_11_0_2.dll",
            EntryPoint = "DeleteString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void DeleteString(IntPtr vectorPtr);
    }
}
