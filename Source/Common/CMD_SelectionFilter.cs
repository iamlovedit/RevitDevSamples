using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /*csdn:https://blog.csdn.net/qq_36253129/article/details/106922199 */
    [Transaction(TransactionMode.Manual)]
    public class CMD_SelectionFilter : IExternalCommand
    {
        private UIDocument m_uidoc;
        private Document m_doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_uidoc = commandData.Application.ActiveUIDocument;
            m_doc = m_uidoc.Document;
            const string m_prompt = "pick a element";
            Element m_pickedElement = m_doc.GetElement(m_uidoc.Selection.PickObject(ObjectType.Element, m_prompt));
            while (true)
            {
                try
                {
                    Element m_element = m_doc.GetElement(m_uidoc.Selection.PickObject(ObjectType.Element,
                        new SelectionFilter(m_pickedElement), m_prompt));
                    TaskDialog.Show("Prompt", $"The name of selected element is {m_element.Name}.");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Debug.Assert(false, e.Message);
                    break;
                }
            }
            return Result.Succeeded;
        }
    }
    public class SelectionFilter : ISelectionFilter
    {
        private readonly Element m_targetElement;
        public SelectionFilter(Element e)
        {
            m_targetElement = e;
        }
        public bool AllowElement(Element elem)
        {
            return elem.Category?.Id == m_targetElement.Category?.Id;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
