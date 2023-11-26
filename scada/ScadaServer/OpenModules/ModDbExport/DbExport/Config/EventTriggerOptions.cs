﻿/*
 * Copyright 2021 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : ModDbExport
 * Summary  : Represents event trigger options
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2021
 * Modified : 2021
 */

using System;
using System.Collections.Generic;

namespace Scada.Server.Modules.DbExport.Config
{
    /// <summary>
    /// Represents event trigger options.
    /// <para>Представляет параметры триггера на событие.</para>
    /// </summary>
    [Serializable]
    internal class EventTriggerOptions : TriggerOptions
    {
        /// <summary>
        /// Gets the trigger type name.
        /// </summary>
        public override string TriggerType => "EventTrigger";

        /// <summary>
        /// Gets the names of the available query parameters.
        /// </summary>
        public override List<string> GetParamNames()
        {
            return new List<string>
            { 
                "dateTime",
                "objNum",
                "kpNum",
                "paramID",
                "cnlNum",
                "oldCnlVal",
                "oldCnlStat",
                "newCnlVal",
                "newCnlStat",
                "checked",
                "userID",
                "descr",
                "data"
            };
        }
    }
}
