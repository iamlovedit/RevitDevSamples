using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /*csdn:https://blog.csdn.net/qq_36253129/article/details/107220379 */
    [Transaction(TransactionMode.Manual)]
    public class CMD_GetLinkedElements : IExternalCommand
    {
        private UIDocument m_uidoc;
        private Document m_doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_uidoc = commandData.Application.ActiveUIDocument;
            m_doc = m_uidoc.Document;

            Application m_app = m_doc.Application;
            while (true)
            {
                try
                {
                    Reference r = m_uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement);
                    foreach (Document doc in m_app.Documents)
                    {
                        if (doc.IsLinked)
                        {
                            ElementId id = r.LinkedElementId;
                            Element e = doc.GetElement(id);
                            if (e != null)
                            {
                                TaskDialog.Show("prompt", $"Id:{e.Id}\nName:{e.Name}\nType:{e.GetType().Name}");
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
            return Result.Succeeded;
        }
    }
}
