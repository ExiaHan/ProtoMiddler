using System.Collections.Generic;
using System.IO;
using System.Linq;
using google.protobuf;
using ProtoBuf.CodeGenerator;

namespace ProtoMiddler.ProtoGen
{
    class ProtoLoader
    {
        readonly bool _addToIncludePath;
        readonly ICollection<string> _includePaths;
        readonly List<string> _paths = new List<string>();

        public ProtoLoader(string path, bool addToIncludePath = true, params string[] includePaths)
            : this(new[] {path}, addToIncludePath, includePaths)
        {
        }

        public ProtoLoader(IEnumerable<string> paths, bool addToIncludePath = true, params string[] includePaths)
        {
            if (paths != null)
            {
                _paths.AddRange(paths);
            }
            _addToIncludePath = addToIncludePath;
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
            var args = string.Join(" ", (_includePaths ?? new string[0])
                .Concat(_addToIncludePath
                    ? _paths.Select(Path.GetDirectoryName)
                    : new string[0])
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
