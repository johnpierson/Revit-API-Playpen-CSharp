﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE03_SetDefaultType : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ///                  TECHNIQUE 3 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ SET DEFAULT TYPE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     Enum.GetValues(typeof(ElementTypeGroup))
                /// 
                /// Demonstrates classes:
                ///     ElementType
                /// 
                /// 
                /// Key methods:
                ///     myElement.GetType()
                ///     doc.SetDefaultFamilyTypeId(
                ///     doc.SetDefaultElementTypeId(
                ///


                if (uidoc.Selection.GetElementIds().Count != 1)
                {
                    MessageBox.Show("Please select ONE Element.");
                    return;
                }

                Element myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());

                if (myElement.GetTypeId() == null)
                {
                    MessageBox.Show("Selected entity does not have types.");
                    return;
                }


                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Set Default Type");

                    if (myElement.GetType() == typeof(FamilyInstance))
                    {
                        doc.SetDefaultFamilyTypeId(myElement.Category.Id, ((FamilyInstance)myElement).Symbol.Id);  //finds and sets the default for editable family categoreis

                    }
                    else
                    {
                        ElementType myElementType = doc.GetElement(myElement.GetTypeId()) as ElementType;
                        foreach (ElementTypeGroup myElementTypeGroup in Enum.GetValues(typeof(ElementTypeGroup)))  //finds and sets the default for system families
                        {
                            if (myElementTypeGroup.ToString() == myElementType.GetType().Name)
                            {
                                doc.SetDefaultElementTypeId(myElementTypeGroup, myElement.GetTypeId());
                            }
                        }
                    }
                    tx.Commit();
                }

                MessageBox.Show("'" + doc.GetElement(myElement.GetTypeId()).Name + "'" + Environment.NewLine + Environment.NewLine + "Is now the defaulf of category." + Environment.NewLine + Environment.NewLine + "'" + myElement.Category.Name + "'");

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_SetDefaultType" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }
}
