using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Common
{
    public class ConfigMgr
    {

        XmlDocument Doc = new XmlDocument();
        string FileName;
        bool doesExist;

        public ConfigMgr(string aFileName)
        {
            FileName = aFileName;

            try
            {
                if (File.Exists(FileName))
                {
                    Doc.Load(aFileName);
                    doesExist = true;
                }
                else
                {
                    Doc.LoadXml(("<?xml version=\"1.0\" encoding=\"euc-kr\"?>\n<configuration>" + "</configuration>"));
                    Doc.Save(aFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadXML()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    Doc.Load(FileName);
                    doesExist = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public string[] GetConfigInfo(string aSection, string aKey, string aDefaultValue)
        {
            // return immediately if the file didn't exist
            if (doesExist == false)
            {
                string[] rtnVal = new string[1];
                rtnVal[0] = "";
                return rtnVal;
            }
            if (aSection == "")
            {
                // if aSection = "" then get all section names
                return getchildren("");
            }
            else if (aKey == "")
            {
                // if aKey = "" then get all keynames for the section
                return getchildren(aSection);
            }
            else
            {
                string[] rtnVal = new string[1];
                rtnVal[0] = getKeyValue(aSection, aKey, aDefaultValue);
                return rtnVal;
            }
        }

        public bool WriteConfigInfo(string aSection, string aKey, string aValue)
        {
            XmlNode node1;
            XmlNode node2;
            //bool rtnVal;
            if (aKey == "")
            {
                // find the section, remove all its keys and remove the section
                node1 = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection);
                // if no such section, return True
                if (node1 == null)
                {
                    return true;
                }
                else
                {
                    // remove all its children
                    node1.RemoveAll();
                    // select its parent ("configuration")
                    node2 = (Doc.DocumentElement).SelectSingleNode("configuration");
                    // remove the section
                    node2.RemoveChild(node1);
                }
            }
            else if (aValue == "")
            {
                // find the section of this key
                node1 = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection);
                // return if the section doesn't exist
                if (node1 == null)
                {
                    return true;
                }
                else
                {
                    // find the key
                    node2 = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection + "/" + aKey);
                    // return true if the key doesn't exist
                    if (node2 == null)
                    {
                        return true;
                    }
                    else
                    {
                        // remove the key
                        if (node1.RemoveChild(node2) == null) return false;
                    }
                }
            }
            else
            {
                // Both the Key and the Value are filled 
                // Find the key
                node1 = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection + "/" + aKey);
                if (node1 == null)
                {
                    // The key doesn't exist: find the section
                    node2 = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection);
                    if (node2 == null)
                    {
                        // Create the section first
                        System.Xml.XmlElement e = Doc.CreateElement(aSection);
                        // Add the new node at the end of the children of ("configuration")
                        node2 = Doc.DocumentElement.AppendChild(e);
                        // return false if failure
                        if (node2 == null)
                        {
                            return false;
                        }
                        else
                        {
                            // now create key and value
                            e = Doc.CreateElement(aKey);
                            e.InnerText = aValue;
                            // Return False if failure
                            if ((node2.AppendChild(e)) == null) return false;
                        }
                    }
                    else
                    {
                        // Create the key and put the value
                        System.Xml.XmlElement e = Doc.CreateElement(aKey);
                        e.InnerText = aValue;
                        node2.AppendChild(e);
                    }
                }
                else
                {
                    // Key exists: set its Value
                    node1.InnerText = aValue;
                }
            }
            
            if (!File.Exists(FileName))
            {
                MessageBox.Show("위치 정보가 맞지 않습니다! 프로젝트 설정을 변경해주십시오!!!");
                return false;
            }
            // Save the document
            Doc.Save(FileName);

            return true;
        }

        private string getKeyValue(string aSection, string aKey, string aDefaultValue)
        {
            XmlNode node;
            node = (Doc.DocumentElement).SelectSingleNode("/configuration/" + aSection + "/" + aKey);
            if (node == null) return aDefaultValue;
            return node.InnerText;
        }

        private string[] getchildren(string aNodeName)
        {
            string[] rtnVal = null;
            XmlNode node;
            try
            {
                // Select the root if the Node is empty
                if (aNodeName == "")
                {
                    node = Doc.DocumentElement;
                }
                else
                {
                    // Select the node given
                    node = Doc.DocumentElement.SelectSingleNode(aNodeName);
                }

                // exit with an empty collection if nothing here
                if (node == null)
                {
                    rtnVal = new string[1];
                    rtnVal[0] = "";
                    return rtnVal;
                }
                // exit with an empty colection if the node has no children
                if (node.HasChildNodes == false)
                {
                    rtnVal = new string[1];
                    rtnVal[0] = "";
                    return rtnVal;
                }
                // get the nodelist of all children
                XmlNodeList nodeList = node.ChildNodes;
                int i;
                rtnVal = new string[nodeList.Count];
                // transform the Nodelist into an ordinary collection
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    rtnVal[i] = nodeList.Item(i).Name;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return rtnVal;
        }

    }
}
