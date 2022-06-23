using System;
using System.Diagnostics.CodeAnalysis;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.ConfigModels;
using ContentPatcher.Framework.Constants;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Migrations
{
    /// <summary>Migrates patches to format version 1.28.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named for clarity.")]
    internal class Migration_1_28: BaseMigration
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Migration_1_28()
            : base(new SemanticVersion(1, 28, 0)) { }

        /// <inheritdoc />
        public override bool TryMigrate(ContentConfig content, [NotNullWhen(false)] out string? error)
        {
            if (!base.TryMigrate(content, out error))
                return false;

            // 1.28 adds Remove(First/Last/All)Occurrence(s) TextOperations
            foreach (PatchConfig? patch in content.Changes)
            {
                if (this.HasAction(patch, PatchType.EditData))
                {
                    foreach (TextOperationConfig? operation in patch.TextOperations)
                    {
                        var operationType = this.GetEnum<TextOperationType>(operation?.Operation);
                        if (operationType is TextOperationType.RemoveFirstOccurrence or TextOperationType.RemoveLastOccurrence or TextOperationType.RemoveAllOccurrences)
                        {
                            error = this.GetNounPhraseError($"using {Enum.GetName(operationType.Value)} {nameof(operation.Operation)}");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
