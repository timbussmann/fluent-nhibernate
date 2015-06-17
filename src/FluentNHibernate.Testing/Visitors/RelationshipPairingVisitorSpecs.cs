using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Utils.Reflection;
using FluentNHibernate.Visitors;
using NUnit.Framework;

namespace FluentNHibernate.Testing.Visitors
{
    [TestFixture(Ignore = true)]
    public class when_mapping_multiple_has_many_relationships_on_same_entity : RelationshipPairingVisitorSpecs
    {
        CollectionMapping oneToManyMapping1;
        CollectionMapping oneToManyMapping2;
        ManyToOneMapping manyToOneMapping1;
        ManyToOneMapping manyToOneMapping2;

        public override void establish_context()
        {
            visitor = new RelationshipPairingVisitor(null);

            oneToManyMapping1 = CollectionMapping.Bag();
            oneToManyMapping1.Set(x => x.Relationship, 0, new OneToManyMapping());
            oneToManyMapping1.Set(x => x.ChildType, 0, typeof(ReferencingClass1));
            oneToManyMapping1.Set(x => x.Member, 0, ReflectionHelper.GetMember<ReferencedClass>(x => x.References1));
            oneToManyMapping1.Set(x => x.Name, 0, "A");
            oneToManyMapping1.ContainingEntityType = typeof(ReferencedClass);
            visitor.ProcessCollection(oneToManyMapping1);

            oneToManyMapping2 = CollectionMapping.Bag();
            oneToManyMapping2.Set(x => x.Relationship, 0, new OneToManyMapping());
            oneToManyMapping2.Set(x => x.ChildType, 0, typeof(ReferencingClass2));
            oneToManyMapping2.Set(x => x.Member, 0, ReflectionHelper.GetMember<ReferencedClass>(x => x.References2));
            oneToManyMapping2.Set(x => x.Name, 0, "B");
            oneToManyMapping2.ContainingEntityType = typeof(ReferencedClass);
            visitor.ProcessCollection(oneToManyMapping2);

            manyToOneMapping1 = new ManyToOneMapping();
            manyToOneMapping1.Set(x => x.Class, 0, new TypeReference(typeof(ReferencedClass)));
            manyToOneMapping1.Set(x => x.Member, 0, ReflectionHelper.GetMember<ReferencingClass1>(c => c.Ref1));
            manyToOneMapping1.Set(x => x.Name, 0, "Z");
            manyToOneMapping1.ContainingEntityType = typeof(ReferencingClass1);
            visitor.ProcessManyToOne(manyToOneMapping1);

            manyToOneMapping2 = new ManyToOneMapping();
            manyToOneMapping2.Set(x => x.Class, 0, new TypeReference(typeof(ReferencedClass)));
            manyToOneMapping2.Set(x => x.Member, 0, ReflectionHelper.GetMember<ReferencingClass2>(c => c.Ref2));
            manyToOneMapping2.Set(x => x.Name, 0, "Y");
            manyToOneMapping2.ContainingEntityType = typeof(ReferencingClass2);
            visitor.ProcessManyToOne(manyToOneMapping2);
        }

        public override void because()
        {
            visitor.Visit(Enumerable.Empty<HibernateMapping>());
        }

        [Test]
        public void should_set_other_side_to_matching_reference()
        {
            oneToManyMapping1.OtherSide.ShouldBeTheSameAs(manyToOneMapping1);
            oneToManyMapping2.OtherSide.ShouldBeTheSameAs(manyToOneMapping2);

            manyToOneMapping1.OtherSide.ShouldBeTheSameAs(oneToManyMapping1);
            manyToOneMapping2.OtherSide.ShouldBeTheSameAs(oneToManyMapping2);
        }

        protected class ReferencingClass1
        {
            public ReferencedClass Ref1 { get; set; }
        }

        protected class ReferencingClass2
        {
            public ReferencedClass Ref2 { get; set; }
        }

        protected class ReferencedClass
        {
            public IEnumerable<ReferencingClass1> References1 { get; set; }

            public IEnumerable<ReferencingClass2> References2 { get; set; }
        }
    }

    public abstract class RelationshipPairingVisitorSpecs : Specification
    {
        protected RelationshipPairingVisitor visitor;
    }
}