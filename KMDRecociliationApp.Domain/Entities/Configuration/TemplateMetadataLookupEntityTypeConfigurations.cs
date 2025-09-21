using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class TemplateMetadataLookupEntityTypeConfigurations : IEntityTypeConfiguration<TemplateMetadataLookup>
    {
        public void Configure(EntityTypeBuilder<TemplateMetadataLookup> builder)
        {
            List<TemplateMetadataLookup> templateMetadataLookups =
            [
                new TemplateMetadataLookup
                {
                    Id = 1,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 0,
                    ColumnKey= "UserType",
                    Description = "User Type"

                },
                new TemplateMetadataLookup
                {
                    Id = 2,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 1,
                    ColumnKey = "Gender",
                    Description = "Gender"

                },
                new TemplateMetadataLookup
                {
                    Id = 3,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 2,
                    ColumnKey = "CountryCode",
                    Description = "Country Code"

                },
                new TemplateMetadataLookup
                {
                    Id = 4,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 3,
                    ColumnKey = "State",
                    Description = "State"

                },
                new TemplateMetadataLookup
                {
                    Id = 5,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 4,
                    ColumnKey = "Association",
                    Description = "Association"

                },
                new TemplateMetadataLookup
                {
                    Id = 6,
                    TemplateName = "UserTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 5,
                    ColumnKey = "Organisations",
                    Description = "Organisations"

                },
                new TemplateMetadataLookup
                {
                    Id = 7,
                    TemplateName = "AssociationTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 0,
                    ColumnKey = "YesNo",
                    Description = "YesNo"

                },
                new TemplateMetadataLookup
                {
                    Id = 8,
                    TemplateName = "AssociationTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 1,
                    ColumnKey = "States",
                    Description = "States"

                },
                new TemplateMetadataLookup
                {
                    Id = 9,
                    TemplateName = "AssociationTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 2,
                    ColumnKey = "Countries",
                    Description = "Countries"

                },
                new TemplateMetadataLookup
                {
                    Id = 10,
                    TemplateName = "AssociationTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 3,
                    ColumnKey = "Organisations",
                    Description = "Organisations"

                },
                new TemplateMetadataLookup
                {
                    Id = 11,
                    TemplateName = "AssociationTemplate",
                    Worksheet = "Metadata",
                    ColumnNumber = 4,
                    ColumnKey = "Association",
                    Description = "Association"

                },

            ];

            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.TemplateName).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.Worksheet).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.ColumnNumber).IsRequired();
            builder.Property(p => p.ColumnKey).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);


            //default data
         //   builder.HasData(templateMetadataLookups);

        }
    }
}

