﻿using System;

namespace UCLouvain.KAOSTools.OmnigraffleExport
{
    public class ExportOptions
    {
        public bool DisplayIdentifiers {
            get;
            set;
        }

        public ExportOptions ()
        {
            DisplayIdentifiers = false;
        }
    }
}

