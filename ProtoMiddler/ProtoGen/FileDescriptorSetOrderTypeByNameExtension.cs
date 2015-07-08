using System;
using System.Collections.Generic;
using System.Linq;
using google.protobuf;

namespace ProtoMiddler.ProtoGen
{
    public static class FileDescriptorSetOrderTypeByNameExtension
    {
        public static IEnumerable<string> OrderByName(this FileDescriptorSet files,
            bool orderByPackageName = false)
        {
            IEnumerable<FileDescriptorProto> enumerable = files.file;
            if (orderByPackageName) enumerable = files.file.OrderBy(o => o.package);
            return enumerable
                .SelectMany(o => o.message_type
                    .OrderBy(t => t.name)
                    .Select(t => Tuple.Create(o.package, t.name)))
                .Select(o => !string.IsNullOrEmpty(o.Item1)
                    ? string.Join(".", o.Item1, o.Item2)
                    : o.Item2);
        }
    }
}
