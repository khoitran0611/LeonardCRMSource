using System;
using System.Reflection;
using System.Reflection.Emit;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.DataLayer.ObjectHelper
{
    public sealed class DynamicObjHelper
    {
        public static TypeBuilder CreateTypeBuilder(
            string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName),
                                       AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        public static void CreateAutoImplementedProperty(
            TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            var setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        public static TypeBuilder DefineObject(TypeBuilder builder, vwFieldNameDataType entity)
        {
            if (entity.IsCheckBox)
            {
                //CreateAutoImplementedProperty(builder, entity.ColumnName, entity.Mandatory ? typeof(bool) : typeof(bool?));
                CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(bool?));
                return builder;
            }
            if (entity.IsDate || entity.IsDateTime)
            {
                //CreateAutoImplementedProperty(builder, entity.ColumnName, entity.Mandatory ? typeof(DateTime) : typeof(DateTime?));
                CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(DateTime?));
                return builder;
            }
            if (entity.IsTime && (entity.Deletable == null || entity.Deletable.Value == false))
            {
                //CreateAutoImplementedProperty(builder, entity.ColumnName,
                //                              entity.Mandatory ? typeof(decimal) : typeof(decimal?));
                CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(TimeSpan?));
                return builder;
            }
            if (entity.IsDecimal || entity.IsCurrency)
            {
                //CreateAutoImplementedProperty(builder, entity.ColumnName,
                //                              entity.Mandatory ? typeof(decimal) : typeof(decimal?));
                CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(decimal?));
                return builder;
            }
            if (entity.IsInteger)
            {
                CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(int?));
                return builder;
            }

            CreateAutoImplementedProperty(builder, entity.ColumnName, typeof(string));
            return builder;

        }

        public static TypeBuilder DefineObject(TypeBuilder builder, vwCustomViewColumn entity)
        {
            if (entity.DataType == (int)DataTypes.CheckBox)
            {
                CreateAutoImplementedProperty(builder, entity.ColumnAlias??entity.ColumnName, typeof(bool?));
                return builder;
            }
            if (entity.DataType == (int)DataTypes.Date)
            {
                CreateAutoImplementedProperty(builder, entity.ColumnAlias ?? entity.ColumnName, typeof(DateTime?));
                return builder;
            }
            if (entity.DataType == (int)DataTypes.Time && (entity.Deletable == null || entity.Deletable.Value == false))
            {
                CreateAutoImplementedProperty(builder, entity.ColumnAlias ?? entity.ColumnName, typeof(TimeSpan?));
                return builder;
            }
            if (entity.DataType == (int)DataTypes.Decimal || entity.IsCurrency)
            {
                CreateAutoImplementedProperty(builder, entity.ColumnAlias ?? entity.ColumnName, typeof(decimal?));
                return builder;
            }
            if (entity.DataType == (int)DataTypes.Integer)
            {
                CreateAutoImplementedProperty(builder, entity.ColumnAlias ?? entity.ColumnName, typeof(int?));
                return builder;
            }

            CreateAutoImplementedProperty(builder, entity.ColumnAlias ?? entity.ColumnName, typeof(string));
            return builder;

        }
    }
}
