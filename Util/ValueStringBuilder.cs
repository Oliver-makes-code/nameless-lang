using System.Numerics;

namespace Lang.Util;

public interface IntoValueString {
    public ValueString IntoValueString();
}

public static class IntoValueStringExtension {
    public static string PrettyString(this IntoValueString value)
        => value.IntoValueString().PrettyString();
}

public abstract record ValueString : IntoPretty {
    private ValueString() {}

    public record Struct(string Name, string? Type, string? Meta, (string, ValueString)[] Values) : ValueString {
        public override string PrettyString(int indent = 0) {
            var name = Name;
            if (Type != null && Type.Length > 0)
                name += $"<{Type}>";
            if (Meta != null && Meta.Length > 0)
                name += $"[{Meta}]";
            if (Values.Length == 0)
                return name;

            var s = $"{name}(";
            for (int i = 0; i < Values.Length; i++) {
                s += "\n";
                s += new string(' ', (indent + 1) * 2);
                var value = Values[i];
                s += $"{value.Item1}: {value.Item2.PrettyString(indent + 1)}";
                if (i < Values.Length - 1)
                    s += ",";
            }
            s += "\n";
            s += new string(' ', indent * 2);
            s += ")";
            return s;
        }
    }
    public record Tuple(string Name, string? Type, string? Meta, ValueString[] Values) : ValueString {
        public override string PrettyString(int indent = 0) {
            var name = Name;
            if (Type != null && Type.Length > 0)
                name += $"<{Type}>";
            if (Meta != null && Meta.Length > 0)
                name += $"[{Meta}]";
            if (Values.Length == 0)
                return name;

            if (Values.Length == 1)
                return $"{name}({Values[0].PrettyString(indent)})";

            var s = $"{name}(";
            for (int i = 0; i < Values.Length; i++) {
                s += "\n";
                s += new string(' ', (indent + 1) * 2);
                s += Values[i].PrettyString(indent + 1);
                if (i < Values.Length - 1)
                    s += ",";
            }
            s += "\n";
            s += new string(' ', indent * 2);
            s += ")";
            return s;
        }
    }
    public record Array(ValueString[] Values) : ValueString {
        public override string PrettyString(int indent = 0) {
            if (Values.Length == 0)
                return "[]";

            var s = "[";
            for (int i = 0; i < Values.Length; i++) {
                s += "\n";
                s += new string(' ', (indent + 1) * 2);
                s += Values[i].PrettyString(indent + 1);
                if (i < Values.Length - 1)
                    s += ",";
            }
            s += "\n";
            s += new string(' ', indent * 2);
            s += "]";
            return s;
        }
    }


    public record String(string Value) : ValueString {
        public override string PrettyString(int indent = 0)
            => $"\"{Value}\"";
    }
    public record Ident(string Value) : ValueString {
        public override string PrettyString(int indent = 0)
            => Value.ToString();
    }
    public record Bool(bool Value) : ValueString {
        public override string PrettyString(int indent = 0)
            => Value.ToString();
    }

    public record Number<T>(T Value) : ValueString where T : struct, INumber<T>, INumberBase<T>, IBinaryNumber<T> {
        public override string PrettyString(int indent = 0)
            => Value.ToString();
    }

    public abstract string PrettyString(int indent = 0);
}

public abstract record ValueStringBuilder {
    private ValueStringBuilder() {}

    public record Struct(string Name, string? Type = null, string? Metadata = null) : ValueStringBuilder {
        private readonly List<(string, ValueString)> Values = [];

        public Struct Field(string name, IntoValueString value)
            => ValueField(name, value.IntoValueString());

        public Struct ArrayField(string name, IntoValueString[] values)
            => ArrayValueField(name, [..values.Select(it => it.IntoValueString())]);

        public Struct ValueField(string name, ValueString value) {
            Values.Add((name, value));
            return this;
        }

        public Struct ArrayValueField(string name, ValueString[] values)
            => ValueField(name, new ValueString.Array(values));

        public Struct StringField(string name, string value)
            => ValueField(name, new ValueString.String(value));

        public Struct IdentField(string name, string value)
            => ValueField(name, new ValueString.Ident(value));
        
        public Struct BoolField(string name, bool value)
            => ValueField(name, new ValueString.Bool(value));

        public Struct NumberField<T>(string name, T value) where T : struct, INumber<T>, INumberBase<T>, IBinaryNumber<T>
            => ValueField(name, new ValueString.Number<T>(value));

        public override ValueString Build()
            => new ValueString.Struct(Name, Type, Metadata, [..Values]);
    }

    public record Tuple(string Name, string? Type = null, string? Metadata = null) : ValueStringBuilder {
        private readonly List<ValueString> Values = [];

        public Tuple Field(IntoValueString value)
            => ValueField(value.IntoValueString());

        public Tuple ArrayField(IntoValueString[] values)
            => ArrayValueField([..values.Select(it => it.IntoValueString())]);

        public Tuple ValueField(ValueString value) {
            Values.Add(value);
            return this;
        }

        public Tuple ArrayValueField(ValueString[] values)
            => ValueField(new ValueString.Array(values));

        public Tuple StringField(string value)
            => ValueField(new ValueString.String(value));

        public Tuple IdentField(string value)
            => ValueField(new ValueString.Ident(value));
        
        public Tuple BoolField(bool value)
            => ValueField(new ValueString.Bool(value));

        public Tuple NumberField<T>(T value) where T : struct, INumber<T>, INumberBase<T>, IBinaryNumber<T>
            => ValueField(new ValueString.Number<T>(value));

        public override ValueString Build()
            => new ValueString.Tuple(Name, Type, Metadata, [..Values]);
    }

    public abstract ValueString Build();
}
