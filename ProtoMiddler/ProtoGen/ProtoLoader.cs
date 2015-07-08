using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using google.protobuf;
using ProtoBuf.CodeGenerator;

namespace ProtoMiddler.ProtoGen
{
    class ProtoLoader
    {
        readonly ICollection<string> _includePaths;
        readonly List<string> _paths = new List<string>();

        public ProtoLoader(string path, params string[] includePaths)
        {
            _paths.Add(path);
            _includePaths = includePaths;
        }

        public ProtoLoader(IEnumerable<string> paths, params string[] includePaths)
        {
            _paths.AddRange(paths);
            _includePaths = includePaths;
        }

        public void AddPath(string path)
        {
            _paths.Add(path);
        }

        public void AddIncludePath(string path)
        {
            _includePaths.Add(path);
        }

        public FileDescriptorSet LoadTypes(Stream ms = null)
        {
            var files = new FileDescriptorSet();
            var args = string.Join(" ", _includePaths
                .Where(Directory.Exists)
                .Select(o => o.ToLowerInvariant())
                .Distinct()
                .Select(o => "-I " + o));
            using (var sr = ms != null ? new StreamWriter(ms) : StreamWriter.Null)
            {
                foreach (var path in _paths
                    .Where(File.Exists)
                    .Select(o => o.ToLowerInvariant())
                    .Distinct())
                {
                    InputFileLoader.Merge(files, path, sr, args);
                }
            }
            return files;
        }
    }
}
