using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Graphics;
using OpenTK;

namespace COG.Dredger.Rendering
{
    public class MeshRenderer
    {
        private ProgramManager m_programs;

        public MeshRenderer(ProgramManager programs)
        {
            m_programs = programs;
        }

        public void Render(IMesh mesh, Matrix4 model)
        {
            m_programs.CurrentProgram.SetUniformMatrix4("model", model);
            mesh.Render(m_programs.CurrentProgram);
        }
    }
}
