﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Tests
{
    internal static class StringsMatchingNonRandomizedHashCode
    {
        /// <summary>
        /// A list of strings that all have the same hashcode (-1038491930) using the legacy non randomized string hash code algorithm
        /// </summary>
        internal static readonly IEnumerable<string> Data = new string[] {
            "95e85f8e-67a3-4367-974f-dd24d8bb2ca2",
            "eb3d6fe9-de64-43a9-8f58-bddea727b1ca",
            "9b9bc0c2-5d25-4292-880c-d54bb48c2f37",
            "f18b7e1a-8096-4346-93fa-e3364b6b0733",
            "60b03506-0199-4b8e-8c67-6537334dca0e",
            "cce5ede8-63f9-484f-b4e7-b99dc2d91443",
            "2325db9d-ab54-44fb-a9c2-176c5d288d4f",
            "46cf9c95-ecef-4b8b-92f7-7bee4fa27e46",
            "b5bd9e71-47ba-487d-b8a6-9215a4fecf7f",
            "ad9959d7-65ac-4411-84cd-716a029c87e5",
            "b55428aa-ef81-4ad5-9e37-6b5a8ca9c0e1",
            "5158d6d2-7d4e-4daa-a360-2b4b26756b90",
            "da014f41-88fb-410c-953a-a9b1dfa3a757",
            "5aec72a1-1df3-4e82-a062-7a75f92541d2",
            "9302fa63-19dc-4a9d-9a4d-7157206f022e",
            "59597f29-523d-4fe9-93ed-1bdfb9b08cc9",
            "187fc738-b8de-4df6-ad74-a4f80a75c342",
            "94dd8708-078b-4aee-9f6a-9e0fbf92b80a",
            "470b67ee-2064-4514-9e27-625f8b56b4da",
            "7306a445-8cec-439b-815e-1c19eb508321",
            "e4965b74-5f66-4d7c-9466-3e8a1877c3ec",
            "b8496759-d960-4ba8-9730-579d1d0ed239",
            "536632d6-459e-4ce2-be87-f21a918733fe",
            "e50e0ea6-beab-4fa5-a9f4-9c96b12c139e",
            "5d9c533b-2b3b-4e95-a1c7-fe69c4327618",
            "1b9f655b-09ea-4cf0-91a5-cda8766f13e4",
            "66cc34f5-b87c-45a9-b113-87d6c9d25cd5",
            "4a9b743f-e055-4be4-9662-05af93470538",
            "e0577593-fd31-4898-a88c-3950d0b19b72",
            "ce1d0443-b80b-492e-8b3f-27e98f5e3e9a",
            "05720b31-5ff1-4724-823f-77922031ac01",
            "68286f1b-db7a-4e3d-957c-63f2568235cd",
            "49e291eb-f684-4925-93c4-6c7185366ab8",
            "4de12acb-b914-46b3-9512-ab98691d63d3",
            "55ff0365-5569-4735-a277-2c33250f0f07",
            "720f0ffb-db80-484a-a595-9010ae049af6",
            "e42648ac-35a9-47eb-aab8-ae2b83cf589a",
            "913148cc-d3f1-481e-a79e-2c7d7df8ea89",
            "02ff9719-1b0b-40d1-bc54-d73db9adbf75",
            "bd58c566-8770-4418-8764-3e1297465a02",
            "5ceac45b-32de-45cd-8b0f-e1c2f682de68",
            "5b89bcca-92db-4223-852b-799ea656a2b5",
            "5386efa9-dce3-434a-9ec0-f04c68893ea2",
            "b9b8baf8-91c3-45e9-b80e-7d2826832332",
            "07206b0c-d1b7-4608-b391-dd9697224d04",
            "851ad5d2-8a2e-459f-ac71-5b1267de7fb7",
            "71ca867c-2caa-48d4-86b2-388cc882e214",
            "e4abda6d-0240-4ead-8738-b86fa8f10737",
            "31ec579b-ac0b-4c18-9b19-3275dbab095d",
            "160a8f1e-72fb-41e1-a5ee-51b7d8b862b9",
            "4c2c3a69-0e15-4e9f-9c4f-46b4d5ba51cc",
            "2adfa71a-23dd-4f2b-b37f-32a45047f97f",
            "997e2a2f-fb2f-4195-9a3e-e32b9757f66f",
            "eca9216f-fc27-4c50-9d8e-4f36fadb141c",
            "f2e1bc76-d0d6-4767-8677-3de239ccb369",
            "19d66436-f554-4514-810c-917831d42bae",
            "0740442c-13d0-4ccc-8471-40519ab592c8",
            "71a10371-f9cb-4db1-9b34-33de5a5fdf6f",
            "4b2802ce-6a90-41be-80da-ca5d74db3dee",
            "14e333d5-9296-4ac1-bf0b-b2da050b8218",
            "8a8f0c91-b73c-41e9-b0e2-36c4dc44c272",
            "e578a3af-c8a8-4677-a518-2c55c55311c3",
            "7a1bece9-edac-4ef9-b60d-ea044e6d5213",
            "80e4c8f3-6fbc-49fc-a78f-68a2c008a5ee",
            "5a92d9a1-ccc2-40ad-b16d-fa76792e6136",
            "ef7041ef-768c-4120-b939-d51faf825037",
            "b7e72bbf-d3f1-4e81-9e05-f37a461cb1ca",
            "afd217a5-c324-4b6c-88cb-5ddfe44b71ba",
            "aa497d73-e054-4803-a3a1-c2628f2cda1c",
            "a85fc2b5-b3b4-4448-ab26-f22fc611a2f6",
            "6369c1a4-e037-4813-8cf6-721ba190e216",
            "5f465559-9cef-4bb7-b6c9-776b19832367",
            "9e30427d-da1f-41d9-bc5f-be7bf905b13d",
            "17f75a2e-2c24-410f-bb9c-45b0190eb71d",
            "22d28aee-bb8d-46a4-8da6-da530aba66b2",
            "e23f24d0-a49a-43a5-98dd-9aa47935776b",
            "9bb540e7-d2a5-429d-86c9-4e22704d1adc",
            "30294db6-4fda-4d98-bbdd-b6fe0903196a",
            "68cb69a7-c672-4076-ad56-1ba635bfe923",
            "67ba1ecc-a33c-4eca-82f5-2fa3c5c63b95",
            "2e65193c-0040-4bea-8984-9fadcc16f673",
            "a2ddd2d6-0d3c-46d7-990d-6fd545e687ae",
            "0775d627-55f6-427a-a1e0-8711d00cf186",
            "bac81b76-4166-4c2d-8fda-81ea89ab98a4",
            "24c8fe6b-379a-401f-bda7-2d6fb0224195",
            "e628506c-a2d6-49b9-b8ac-d44a505d7bd2",
            "d4098a97-f749-4e80-bbd9-5a3ee2799150",
            "1dd4c357-6b4b-4c79-8265-61c7ed933c29",
            "204bd760-cb3b-42e1-9229-1ba75e64210a",
            "dac72303-82ba-43b0-a702-116556aeaa67",
            "b0a93178-586f-436d-b73e-daf631de81b5",
            "60792704-3eab-4d80-873f-44a58ca45dd1",
            "602c80ce-6d82-4368-8099-8be4beabe628",
            "1c0a59a4-1f5b-4c8b-ae44-0d2642016eba",
            "0fd9cee5-3ebf-4bea-b171-5aa9faac4973",
            "6f51bd07-2985-4077-ba11-dfd6032aff39",
            "5c0d55c8-8641-4796-8781-12a0bed9ef9d",
            "69098dfe-e5a6-41bd-913c-bb43b5a94661",
            "7d1f34de-1368-4cd8-939c-9a273a5ac89b",
            "70c65496-16b8-42b2-b600-f5848990fe89",
            "899dd758-8f74-4fa7-860b-e70deb15f736",
            "bd8c8608-4056-414c-9100-6591a7d97f7f",
        };
    }
}
