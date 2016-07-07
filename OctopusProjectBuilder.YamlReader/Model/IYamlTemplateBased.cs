using System.ComponentModel;
using OctopusProjectBuilder.YamlReader.Model.Templates;

namespace OctopusProjectBuilder.YamlReader.Model
{
    public interface IYamlTemplateBased
    {
        [DefaultValue(null)]
        YamlTemplateReference UseTemplate { get; set; }

        void ApplyTemplate(YamlTemplates templates);
    }
}