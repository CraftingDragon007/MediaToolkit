using MediaToolkit.Core.Infrastructure;
using MediaToolkit.Core.Services;
using NUnit.Framework;

namespace MediaToolkit.Core.Test
{
    [TestFixture]
    public class MetadataServiceTests
    {
        private MetadataService? _metadataService;
        private string? _testDir;
        private string? _videoPath;

        [OneTimeSetUp]
        public Task Setup()
        {
            this._metadataService = new MetadataService(new FFprobeServiceConfiguration());

            this._testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).FullName + "/TestResults";
            this._videoPath = this._testDir + "/BigBunny.m4v";

            this._metadataService.OnMetadataProcessedEventHandler += (_, args) =>
            {
                Console.WriteLine(args.Metadata.RawMetaData);
            };

            if (File.Exists(this._videoPath))
            {
                return Task.CompletedTask;
            }

            Directory.CreateDirectory(this._testDir);
            return Task.CompletedTask;
            //Assembly currentAssembly = Assembly.GetExecutingAssembly();

            /*using (Stream embeddedVideoStream = currentAssembly.GetManifestResourceStream(TestVideoResourceId))
            using (FileStream fileStream = new FileStream(this.videoPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // ReSharper disable once PossibleNullReferenceException
                await embeddedVideoStream.CopyToAsync(fileStream);
            }*/
        }

        #region Metadata Tests

        [Test]
        public async Task GetMetadataTest()
        {
            //MediaConverterTests tests = new MediaConverterTests();
            //await tests.Setup(false);
            //await tests.Basic_InstructionBuilder_Test();
            //await tests.ConvertToGif_InstructionBuilder_Test();
            //await tests.CropVideo_InstructionBuilder_Test();
            //await tests.Custom_InstructionBuilder_Test();
            //await tests.ExctractOnlineThumbnail_InstructionBuilder_Test();
            //await tests.ExctractThumbnail_InstructionBuilder_Test();
            //await tests.TrimMedia_InstructionBuilder_Test();

            foreach (var file in Directory.GetFiles(this._testDir!))
            {
                IInstructionBuilder custom = new GetMetadataInstructionBuilder()
                {
                    InputFilePath = file,
                };

                await this._metadataService!.ExecuteInstructionAsync(custom);
            }
            
        }


        #endregion

    }
}
