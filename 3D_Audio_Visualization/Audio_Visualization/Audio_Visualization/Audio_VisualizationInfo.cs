using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Audio_Visualization
{
    public class Audio_VisualizationInfo : GH_AssemblyInfo
    {
        public override string Name => "Audio_Visualization";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("8ae5a768-4d3a-42f8-b5cc-2dc20545f3d9");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}