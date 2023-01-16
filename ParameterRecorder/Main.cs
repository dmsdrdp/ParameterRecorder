using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterRecorder
{
    //Занятие 3.3 Запись параметра

    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите элемент");

            foreach (var selectedElement in selectedRef)
            {
                var element = doc.GetElement(selectedElement);

                if (element is Pipe)
                {
                    using (Transaction ts = new Transaction(doc, "Set parameter"))
                    {
                        ts.Start();
                        var familyInstance = element as Pipe;
                        Parameter textPrarmrter = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                        Parameter reservLenght = familyInstance.LookupParameter("Длина с запасом");
                        double reservCoefficient = 1.1; //коэффициент запаса

                        if (textPrarmrter.StorageType == StorageType.Double)
                        {
                            double amount = UnitUtils.ConvertFromInternalUnits(textPrarmrter.AsDouble(), UnitTypeId.Feet);
                            reservLenght.Set(amount * reservCoefficient);
                        }

                        ts.Commit();
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
