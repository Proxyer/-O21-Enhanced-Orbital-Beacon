using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21EnhancedBeacon
{
    public class CompProperties_EnhancedBeacon : CompProperties
    {
        public CompProperties_EnhancedBeacon()
        {
            this.compClass = typeof(Comp_EnhancedBeacon);
        }

        /// <summary>
        /// Sound while on
        /// </summary>
        public SoundDef ambientSound = null;
        /// <summary>
        /// Sound randomly plays every so often.
        /// </summary>
        public SoundDef pingSound = null;
        /// <summary>
        /// Sound plays when powered up.
        /// </summary>
        public SoundDef startupSound = null;
        /// <summary>
        /// Sound plays when powered down.
        /// </summary>
        public SoundDef shutdownSound = null;
    }
}
