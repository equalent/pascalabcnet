﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PascalABCCompiler;
using PascalABCCompiler.Errors;
using PascalABCCompiler.SyntaxTree;

namespace SyntaxVisitors.TypeclassVisitors
{
    public class ReplaceTypeclassVisitor: BaseChangeVisitor
    {
        public ReplaceTypeclassVisitor()
        {

        }

        bool VisitInstanceDeclaration(type_declaration instanceDeclaration)
        {
            var instanceDefinition = instanceDeclaration.type_def as instance_definition;
            if (instanceDefinition == null)
            {
                return false;
            }
            var instanceName = instanceDeclaration.type_name as typeclass_restriction;

            /*
            var instances = typeclassInstanceDeclarations[instanceName.name];
            var restrictedType = instanceName.restriction_args.params_list[0].ToString();
            if (!instances.ContainsKey(restrictedType))
            {
                instances.Add(restrictedType, new List<declaration>());
            }
            foreach (var defBlock in instanceDefinition.body.class_def_blocks)
            {
                instances[restrictedType].AddRange(defBlock.members);
            }*/

            return true;
        }


        bool VisitTypeclassDeclaration(type_declaration typeclassDeclaration)
        {
            var typeclassDefinition = typeclassDeclaration.type_def as typeclass_definition;
            if (typeclassDefinition == null)
            {
                return false;
            }

            var typeclassName = typeclassDeclaration.type_name as typeclass_restriction;

            // TODO: typeclassDefinition.additional_restrictions - translate to usual classes

            var typeclassDefTranslated =
                SyntaxTreeBuilder.BuildClassDefinition(
                    typeclassDefinition.additional_restrictions,
                    null, typeclassDefinition.body.class_def_blocks.ToArray());

            typeclassDefTranslated.attribute = class_attribute.Abstract;
            for (int i = 0; i < typeclassDefTranslated.body.class_def_blocks.Count; i++)
            {
                var cm = typeclassDefTranslated.body.class_def_blocks[i];

                for (int j = 0; j < cm.Count; j++)
                {
                    (cm[j] as function_header)?.proc_attributes.Add(new procedure_attribute("abstract", proc_attribute.attr_abstract));
                    (cm[j] as procedure_header)?.proc_attributes.Add(new procedure_attribute("abstract", proc_attribute.attr_abstract));
                }
            }
            // TODO: add constructor

            var templates = new ident_list();
            templates.source_context = typeclassName.restriction_args.source_context;
            for (int i = 0; i < typeclassName.restriction_args.Count; i++)
            {
                templates.Add((typeclassName.restriction_args[i] as named_type_reference).names[0]);
            }

            var typeclassNameTanslated = new template_type_name(typeclassName.name, templates, typeclassName.source_context);

            /*
            if (typeclassInstanceDeclarations.ContainsKey(typeclassName.name))
            {
                // AddError
            }
            else
            {
                typeclassInstanceDeclarations.Add(typeclassName.name, new Dictionary<string, List<declaration>>());
            }*/

            return true;
        }


        public override void visit(type_declaration _type_declaration)
        {
            if (VisitInstanceDeclaration(_type_declaration))
            {
                return;
            }

            if (VisitTypeclassDeclaration(_type_declaration))
            {
                return;
            }
        }


        public override void visit(procedure_definition _procedure_definition)
        {
            bool isConstrainted = _procedure_definition.proc_header.where_defs != null &&
                _procedure_definition.proc_header.where_defs.defs.Any(x => x is where_typeclass_constraint);
            if (!isConstrainted)
                return;



            foreach (var where in _procedure_definition.proc_header.where_defs.defs)
            {
                var whereC = (where as where_typeclass_constraint).restriction;

                /*
                var instances = typeclassInstanceDeclarations[whereC.name];
                foreach (var instance in instances.Values)
                {
                    (_procedure_definition.proc_body as block).defs.defs.AddRange(instance);
                }

                // substitution template type
                var restricted = whereC.restriction_args.params_list[0].ToString();
                var newType = instances.Keys.First();
                foreach (var param in _procedure_definition.proc_header.parameters.params_list)
                {
                    var type = param.vars_type.ToString();
                    if (type == restricted)
                        param.vars_type = new named_type_reference(newType);
                }

                var func = _procedure_definition.proc_header as function_header;
                if (func != null)
                {
                    var type = func.return_type.ToString();
                    if (type == restricted)
                        func.return_type = new named_type_reference(newType);
                }*/
            }
        }

    }



}
