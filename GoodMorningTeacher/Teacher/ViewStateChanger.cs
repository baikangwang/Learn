using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Teacher
{
    public class ViewStateChanger
    {
        public string Change()
        {
            return string.Empty;
        }

        public string SerializeViewState(string viewState)
        {
            string vsInput = viewState;
            StringBuilder sb=new StringBuilder();
            object vsObject = new LosFormatter().Deserialize(vsInput);
            // -Triplet.First
            // Get Page view state - the First object in the first Triplet
            string tFirst = ParseViewState(((Triplet)vsObject).First, 1);
            sb.AppendLine(string.Format("<page hash=\"{0}\">", tFirst));
            // -Triplet.First - Page hash value
            // -Triplet.Second
            //               .First(Pair)
            //                          .First(ArrayList) - keys
            //                          .Second(ArrayList)- values
            // get the secound object of the first triplet, if the first object of that object is pair
            // it holds user viewstate. the first element in the pair is arraylist of keys and the second element in the pair 
            // is the arraylist of values.
                    sb.AppendLine("<userviewstates>");
            Triplet Second = (Triplet)((Triplet)vsObject).Second;
            if (Second.First is Pair)
            {
                var oPair = (Pair)Second.First;
                var oKeys = (ArrayList)oPair.First;
                var oVals = (ArrayList)oPair.Second;
                for (int i = 0; i < oKeys.Count; i++)
                {
                    sb.AppendLine(string.Format("<userviewstate key=\"{0}\" value=\"{1}\"/>",oKeys[i],oVals[i]));
                }
            }
                    sb.AppendLine("</userviewstates>");
            // -Triplet.First - Page hash value
            // -Triplet.Second
            //               .First(Pair)
            //                          .First(ArrayList) - keys
            //                          .Second(ArrayList)- values
            //               .Third(ArrayList)
            //                                [X](Triplet)
            //                                            .Second(ArrayList) - array that holds the control
            //                                                                 Position in Form controls
            //                                                                 collection.
            //                                            .Third(ArrayList) - array that holds the viewstate
            //                                                                for every control from the upper
            //                                                                array. 
            // get object list. first we get the third object that contains array of triplets
                    sb.AppendLine("<controls>");
                    ArrayList controls = (ArrayList)Second.Third;
            // loop thrhogou the array triplet 
            for (int i = 0; i < controls.Count; i++)
            {
                // get the triplet second and third objects that contain list of control location in the form
                // controls collection and the match entry in the third object contain the control ViewState.
                Triplet control = (Triplet)controls[i];
                ArrayList oArrObjects = (ArrayList)control.Second;
                ArrayList oArrData = (ArrayList)control.Third;
                for (int iCont = 0; iCont < oArrObjects.Count; iCont++)
                {
                    // get the control ID
                    string ContID = (string) oArrObjects[iCont];
                    // Get the control ViewState.
                    Triplet oTrip = (Triplet)oArrData[iCont];
                    sb.AppendLine(string.Format("<control id=\"{0}\" value=\"{1}\"/>", ContID, oTrip));
                }
            }
            sb.AppendLine("</controls>");
            sb.AppendLine("</page>");
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.xml", tFirst));
            using (FileStream fs=File.OpenWrite(file    ))
            {
                byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

                fs.Write(content,0,content.Length);
                fs.Flush(true);
                fs.Close();
            }

            return file;
        }

        private string tabs(int Level)
        {
            string tabs = string.Empty;
            for (int i = 0; i < Level; i++)
            {
                tabs += "-";
            }
            return tabs;
        }

        private string ItemViewState(string tabs, string Item)
        {
            return string.Format("{0}{1}", tabs, Item);
        }

        private string NodeViewState(string Item, string Children)
        {
            return string.Format("{0}->{1}", Item, Children);
        }

        private string ParseViewState(object ViewState, int Level)
        {
            if (ViewState == null)
            {
                return ItemViewState(tabs(Level - 1), "null");
            }
            else if (ViewState is Triplet)
            {
                string children = ParseViewState((Triplet)ViewState, Level);

                return NodeViewState(tabs(Level - 1) + "Triplet", tabs(Level - 1) + children);
            }
            else if (ViewState is Pair)
            {
                string children = ParseViewState((Pair)ViewState, Level);
                return NodeViewState(tabs(Level - 1) + "Pair", tabs(Level - 1) + children);
            }
            else if (ViewState is ArrayList)
            {
                string children = ParseViewState((IEnumerable)ViewState, Level);

                return NodeViewState(tabs(Level - 1) + "ArrayList", tabs(Level - 1) + children);
            }
            else if (ViewState.GetType().IsArray)
            {
                string children = ParseViewState((IEnumerable)ViewState, Level);
                return NodeViewState(tabs(Level - 1) + "Array", tabs(Level - 1) + children);
            }
            else if (ViewState is string)
            {
                return ItemViewState(tabs(Level - 1), ViewState.ToString());
            }
            else if (ViewState.GetType().IsPrimitive)
            {
                return ItemViewState(tabs(Level - 1), ViewState.ToString());
            }
            else
            {
                return ItemViewState(tabs(Level - 1), ViewState.GetType().ToString());
            }
        }

        private string ParseViewState(Triplet ViewState, int Level)
        {
            if (ViewState == null)
                return string.Empty;

            string first = ParseViewState(ViewState.First, Level + 1);
            string second = ParseViewState(ViewState.Second, Level + 1);
            string third = ParseViewState(ViewState.Third, Level + 1);

            return tabs(Level - 1) + first + second + third;
        }

        private string ParseViewState(Pair ViewState, int Level)
        {
            if (ViewState == null)
                return string.Empty;
            
            string first = ParseViewState(ViewState.First, Level + 1);
            string second = ParseViewState(ViewState.Second, Level + 1);
            return tabs(Level - 1) + first + second;
        }

        private string ParseViewState(IEnumerable ViewState, int Level)
        {
            if (ViewState == null)
                return string.Empty;
            
            string items = string.Empty;
            foreach (object item in ViewState)
            {
                items += ParseViewState(item, Level + 1);
            }

            return tabs(Level - 1) + items;
        }
    }
}
