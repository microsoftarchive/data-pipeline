// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Layouts;

namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    [NLogConfigurationItem]
    public sealed class ElastisearchParameterInfo
    {
        public ElastisearchParameterInfo()
            : this(null, null)
        {
        }

        public ElastisearchParameterInfo(string name, Layout layout)
        {
            this.Name = name;
            this.Layout = layout;
        }

        [RequiredParameter]
        public string Name { get; set; }

        [RequiredParameter]
        public Layout Layout { get; set; } 
    }
}
