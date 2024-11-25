﻿using System;
using JetBrains.Annotations;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Vostok.Hercules.Client.Abstractions;
using Vostok.OpenTelemetry.Exporter.Hercules.Builders;

namespace Vostok.OpenTelemetry.Exporter.Hercules;

[PublicAPI]
public sealed class HerculesLogExporter : BaseExporter<LogRecord>
{
    private readonly IHerculesSink sink;
    private readonly Func<HerculesLogExporterOptions> optionsProvider;
    private Resource? resource;

    public HerculesLogExporter(IHerculesSink sink, Func<HerculesLogExporterOptions> optionsProvider)
    {
        this.sink = sink;
        this.optionsProvider = optionsProvider;
    }

    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        resource ??= ParentProvider.GetResource();
        var options = optionsProvider();

        foreach (var logRecord in batch)
            sink.Put(options.Stream, builder => builder.BuildLogRecord(logRecord, resource));

        return ExportResult.Success;
    }
}