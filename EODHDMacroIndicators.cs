﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using QuantConnect.Data;
using QuantConnect.Data.UniverseSelection;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuantConnect.DataSource;

/// <summary>
/// EODHDMacroIndicators data type
/// </summary>
public class EODHDMacroIndicators : BaseDataCollection
{
    private static readonly EODHDMacroIndicator _factory = new();

    /// <summary>
    /// Return the URL string source of the file. This will be converted to a stream
    /// </summary>
    /// <param name="config">Configuration object</param>
    /// <param name="date">Date of this source file</param>
    /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
    /// <returns>String URL of source file.</returns>
    public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
    {
        var country = config.Symbol.Value.Split('/')[0];
        return new SubscriptionDataSource(
            Path.Combine(
                Globals.DataFolder,
                "alternative",
                "eodhd",
                "macroindicators",
                $"{country.ToLowerInvariant()}.csv"
            ),
            SubscriptionTransportMedium.LocalFile,
            FileFormat.FoldingCollection
        );
    }

    /// <summary>
    /// Parses the data from the line provided and loads it into LEAN
    /// </summary>
    /// <param name="config">Subscription configuration</param>
    /// <param name="line">Line of data</param>
    /// <param name="date">Date</param>
    /// <param name="isLiveMode">Is live mode</param>
    /// <returns>New instance</returns>
    public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
    {
        return _factory.Reader(config, line, date, isLiveMode);
    }

    /// <summary>
    /// Clones the data
    /// </summary>
    /// <returns>A clone of the object</returns>
    public override BaseData Clone()
    {
        return new EODHDMacroIndicators
        {
            Symbol = Symbol,
            Time = Time,
            EndTime = EndTime,
            Data = Data?.ToList(point => point.Clone())
        };
    }

    /// <summary>
    /// Indicates whether the data source is tied to an underlying symbol and requires that corporate indicators be applied to it as well, such as renames and delistings
    /// </summary>
    /// <returns>false</returns>
    public override bool RequiresMapping()
    {
        return _factory.RequiresMapping();
    }

    /// <summary>
    /// Indicates whether the data is sparse.
    /// If true, we disable logging for missing files
    /// </summary>
    /// <returns>true</returns>
    public override bool IsSparseData()
    {
        return _factory.IsSparseData();
    }

    /// <summary>
    /// Converts the instance to string
    /// </summary>
    public override string ToString()
    {
        return $"[{string.Join(",", Data.Select(data => data.ToString()))}]";
    }

    /// <summary>
    /// Gets the default resolution for this data and security type
    /// </summary>
    public override Resolution DefaultResolution()
    {
        return _factory.DefaultResolution();
    }

    /// <summary>
    /// Gets the supported resolution for this data and security type
    /// </summary>
    public override List<Resolution> SupportedResolutions()
    {
        return _factory.SupportedResolutions();
    }
}