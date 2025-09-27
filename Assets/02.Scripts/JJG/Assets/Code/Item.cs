using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJG
{
    public class Item
    {
        public enum ItemType
        {

            칠게,

            농게,

            갯게,

            붉은발말똥게,

            홍합,

            갯고둥,

            총알고둥,

            맛조개,

            개조개,

            갯지렁이,
        }
        public ItemType itemType;
        public int amount;
    }
}