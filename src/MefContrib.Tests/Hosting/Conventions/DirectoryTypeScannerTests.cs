namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using MefContrib.Tests;

    using NUnit.Framework;

    public class DirectoryTypeScannerTests
    {
        [Test]
        public void Ctor_should_throw_argumentnullexception_when_called_with_null_path()
        {
            var exception =
                Catch.Exception(() => new DirectoryTypeScanner(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argumentoutofrangeexception_when_called_with_empty_path()
        {
            var exception =
                Catch.Exception(() => new DirectoryTypeScanner(string.Empty));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_throw_directorynotfoundexception_when_called_with_non_existing_path()
        {
            var exception =
                Catch.Exception(() => new DirectoryTypeScanner("fodofdf"));

            exception.ShouldBeOfType<DirectoryNotFoundException>();
        }

        [Test]
        public void Ctor_should_persist_path_to_path_property()
        {
            var knownPath =
                Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            var scanner =
                new DirectoryTypeScanner(knownPath);

            scanner.Path.ShouldEqual(knownPath);
        }

        [Test]
        public void GetTypes_should_return_public_types_from_all_available_assemblies_in_path()
        {
            var scanner =
                CreateDefaultDirectoryTypeScanner();

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(2);
        }

        [Test]
        public void GetTypes_should_return_empty_enumerable_when_no_assemblies_were_found_in_path()
        {
            var tempDirectoryPath =
                Path.Combine(Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            Directory.CreateDirectory(tempDirectoryPath);

            var scanner =
                new DirectoryTypeScanner(tempDirectoryPath);

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(0);
        }

        [Test]
        public void GetTypes_should_return_types_from_files_with_accepted_extensions()
        {
            var scanner =
                CreateDefaultDirectoryTypeScanner();

            var files =
                Directory.GetFiles(scanner.Path);

            File.Move(
                files[0],
                files[0].Replace(".dll", ".exe"));

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(2);
        }

        [Test]
        public void GetTypes_should_not_return_types_from_files_without_accepted_extension()
        {
            var scanner =
                CreateDefaultDirectoryTypeScanner();

            var files =
                Directory.GetFiles(scanner.Path);

            File.Move(
                files[0],
                files[0].Replace(".dll", ".jpg"));

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(1);
        }

        public static DirectoryTypeScanner CreateDefaultDirectoryTypeScanner()
        {
            var tempDirectoryPath =
                Path.Combine(Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var directory =
                Directory.CreateDirectory(tempDirectoryPath);

            for (var i = 0; i < 2; i++)
            {
                var assemblyName =
                    Path.Combine(directory.FullName,
                    string.Concat(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), ".dll"));

                var results =
                    CSharpAssemblyFactory.Compile(
                        @"
                        public class Foo { }
                        class Bar { }
                        public interface IFoo { }
                        interface IBar { }
                        public abstract class Baz { }
                    ", assemblyName);
            }

            return new DirectoryTypeScanner(tempDirectoryPath);
        }
    }
}