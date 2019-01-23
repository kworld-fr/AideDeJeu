﻿using AideDeJeu.ViewModels;
using AideDeJeuLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AideDeJeuLib
{
    public class MagicItems : FilteredItems
    {
        public override FilterViewModel GetNewFilterViewModel()
        {
            return new VFMagicItemFilterViewModel();
        }
    }
}
