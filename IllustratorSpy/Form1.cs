using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Illustrator;
using Application = Illustrator.Application;

namespace IllustratorSpy
{
    public partial class Form1 : Form
    {

    private Application _app;

    private Illustrator.Document _doc;

    public Form1()
        {
            InitializeComponent();

      treeView1.Click += TreeView1_Click;
        }

    private void TreeView1_Click(object sender, EventArgs e)
    {
      switch(treeView1.SelectedNode.ToolTipText)
      {
        case "Document":
          Document document = treeView1.SelectedNode.Tag as Document;
          propertyGrid1.SelectedObject = document;
          break;
        case "Layer":
          Layer layer = treeView1.SelectedNode.Tag as Layer;
          propertyGrid1.SelectedObject = layer;
          break;
        case "Group":
          GroupItem groupItem = treeView1.SelectedNode.Tag as GroupItem;
          propertyGrid1.SelectedObject = groupItem;
          break;
        case "TextFrame":
          TextFrame textFrame= treeView1.SelectedNode.Tag as TextFrame;
          propertyGrid1.SelectedObject = textFrame;
          break;
        case "TextRange":
          TextRange textRange = treeView1.SelectedNode.Tag as TextRange;
          propertyGrid1.SelectedObject = textRange;
          break;
      }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var d = new OpenFileDialog();
      if (d.ShowDialog() == DialogResult.OK)
      {
        _app = new Application();
        _app.UserInteractionLevel = AiUserInteractionLevel.aiDontDisplayAlerts;
        _doc = _app.Open(d.FileName);
        treeView1.Nodes.Clear();
        var docNode = treeView1.Nodes.Add(_doc.Name);
        Visit(_doc, docNode);
      }
    }

    public void Visit(Document document, TreeNode node)
    {
      node.Tag = document;
      node.Text = document.Name;
      node.ToolTipText = "Document";
      foreach(Layer layer in document.Layers)
      {
        var n = node.Nodes.Add(layer.Name);
        Visit(layer, n);
      }
    }

    public void Visit(Layer layer, TreeNode node)
    {
      foreach(GroupItem groupItem in layer.GroupItems)
      {
        var n = node.Nodes.Add(groupItem.Name);
        Visit(groupItem, n);
      }
      foreach(TextFrame textFrame in layer.TextFrames)
      {
        var n = node.Nodes.Add(textFrame.Name);
        Visit(textFrame, n);
      }
    }

    public void Visit(TextFrame textFrame, TreeNode node)
    {
      node.Tag = textFrame;
      node.Text = textFrame.Name;
      node.ToolTipText = "TextFrame";
      foreach(TextRange textRange in textFrame.TextRanges)
      {
        var n = node.Nodes.Add(textRange.Contents);
        Visit(textRange, node);
      }
    }

    public void Visit(TextRange textRange, TreeNode node)
    {
      node.Tag = textRange;
      node.Text = textRange.Contents;
      node.ToolTipText = "TextRange";
      foreach(TextRange range in textRange.TextRanges)
      {
        var n = node.Nodes.Add(range.Contents);
        Visit(range, node);
      }
    }


    public bool Visit(GroupItem group, TreeNode node)
    {
      foreach (TextFrame textFrame in group.TextFrames)
      {
        Visit(textFrame, node);
      }
      foreach (GroupItem groupItem in group.GroupItems)
      {
        Visit(groupItem, node);
      }

      return true;
    }
  }
}
