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
    /*csdn:https://blog.csdn.net/qq_36253129/article/details/107234547 */
    [Transaction(TransactionMode.Manual)]
    public class CMD_MakeWallsDisjoin : IExternalCommand
    {
        private UIDocument m_uidoc;
        private Document m_doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_uidoc = commandData.Application.ActiveUIDocument;
            m_doc = m_uidoc.Document;

            WallUpdater m_wallUpdater = new WallUpdater(commandData.Application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(m_wallUpdater, m_doc);
            UpdaterRegistry.AddTrigger(m_wallUpdater.UpdaterId, m_wallUpdater.ElementFilter, m_wallUpdater.ChangeType);
            return Result.Succeeded;
        }
    }
    public class WallUpdater : IUpdater
    {
        public UpdaterId UpdaterId { get; }
        public ElementFilter ElementFilter { get; } = new ElementClassFilter(typeof(Wall));
        public ChangeType ChangeType { get; } = Element.GetChangeTypeElementAddition();
        public WallUpdater(AddInId addinId)
        {
            UpdaterId = new UpdaterId(addinId, addinId.GetGUID());
        }
        public void Execute(UpdaterData data)
        {
            Document m_doc = data.GetDocument();
            IEnumerable<Wall> m_addedWalls = data.GetAddedElementIds().Select(m_doc.GetElement).OfType<Wall>();
            foreach (var wall in m_addedWalls)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (WallUtils.IsWallJoinAllowedAtEnd(wall, i))
                    {
                        WallUtils.DisallowWallJoinAtEnd(wall, i);
                    }
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "Disallow Walls Join";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.InteriorWalls | ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return UpdaterId;
        }

        public string GetUpdaterName()
        {
            return "WallUpdater";
        }
    }

}
